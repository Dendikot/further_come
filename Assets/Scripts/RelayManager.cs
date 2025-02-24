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
            hostButton.onClick.AddListener(()=> _ = CreateRelayAsync());
            joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));
        }


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

    async void JoinRelay(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
}
