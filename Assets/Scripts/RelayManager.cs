using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.UI;
using Unity.Services.Authentication;
using System.Threading;
using System;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TextMeshProUGUI codeText;

    private async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        hostButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(() => _ = CreateRelayAsync());
        joinButton.onClick.AddListener(() => _ = JoinRelayAsync(joinInput.text));

    }

    private static SemaphoreSlim _relayLock = new SemaphoreSlim(1, 1);
    private CancellationTokenSource _cancellationTokenSource;

    async Task CreateRelayAsync()
    {
        if (!_relayLock.Wait(0))
        {
            Debug.LogWarning("CreateRelayAsync is already running. Restarting the relay...");
        
            // Cancel the previous relay creation process
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }
        else
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        try
        {
            // If a previous instance exists, shut it down
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Stopping previous host...");
                NetworkManager.Singleton.Shutdown();
            }

            // Start new relay creation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codeText.text = joinCode;

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // Check if the task has been canceled before starting the new host
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                Debug.LogWarning("Relay creation was canceled before starting the new host.");
                return;
            }

            NetworkManager.Singleton.StartHost();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create relay: {ex.Message}");
        }
        finally
        {
            _relayLock.Release();
        }
    }

    private static SemaphoreSlim _joinLock = new SemaphoreSlim(1, 1);
    private CancellationTokenSource _joinCancellationTokenSource;

    async Task JoinRelayAsync(string joinCode)
    {
        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogError("Join code is null or empty. Cannot join relay.");
            return;
        }

        if (!_joinLock.Wait(0))
        {
            Debug.LogWarning("JoinRelayAsync is already running. Cancelling previous attempt and restarting...");

            // Cancel the previous join attempt
            _joinCancellationTokenSource?.Cancel();
            _joinCancellationTokenSource = new CancellationTokenSource();
        }
        else
        {
            _joinCancellationTokenSource = new CancellationTokenSource();
        }

        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            Debug.LogWarning("Client is already running. Cannot join again.");
            _joinLock.Release();
            return;
        }

        try
        {
            Debug.Log($"Attempting to join relay with code: {joinCode}");

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // If a new join request was made, cancel this one
            if (_joinCancellationTokenSource.Token.IsCancellationRequested)
            {
                Debug.LogWarning("Join operation was canceled before completion.");
                return;
            }

            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            Debug.Log("Successfully joined relay.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to join relay: {ex.Message}");
        }
        finally
        {
            _joinLock.Release();
        }
    }
}
