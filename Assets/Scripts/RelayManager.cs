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

public class RelayManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TextMeshProUGUI codeText;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(()=> JoinRelay(joinInput.text));
    }

    async void CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        codeText.text= "THE CODE:\n" + joinCode;

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();
    }

    async void JoinRelay(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
}
