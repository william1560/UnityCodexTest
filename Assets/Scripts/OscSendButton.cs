using extOSC;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(OSCTransmitter))]
public class OscSendButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private InputField messageInput;
    [SerializeField] private Button sendButton;

    [Header("OSC Settings")]
    [SerializeField] private string oscAddress = "/unity/message";
    [SerializeField] private string remoteHost = "127.0.0.1";
    [SerializeField] private int remotePort = 7000;

    private OSCTransmitter transmitter;

    private void Awake()
    {
        transmitter = GetComponent<OSCTransmitter>();
        if (transmitter == null)
        {
            transmitter = gameObject.AddComponent<OSCTransmitter>();
        }

        ApplyTransmitterSettings();

        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendOscMessage);
        }
    }

    private void OnDestroy()
    {
        if (sendButton != null)
        {
            sendButton.onClick.RemoveListener(SendOscMessage);
        }
    }

    private void OnValidate()
    {
        if (transmitter == null)
        {
            transmitter = GetComponent<OSCTransmitter>();
            if (transmitter == null)
            {
                transmitter = gameObject.AddComponent<OSCTransmitter>();
            }
        }

        ApplyTransmitterSettings();
    }

    private void ApplyTransmitterSettings()
    {
        if (transmitter != null)
        {
            transmitter.RemoteHost = remoteHost;
            transmitter.RemotePort = remotePort;
        }
    }

    public void SendOscMessage()
    {
        if (transmitter == null)
        {
            return;
        }

        string payload = messageInput != null ? messageInput.text : string.Empty;

        var message = new OSCMessage(oscAddress);
        message.AddValue(OSCValue.String(payload));

        transmitter.Send(message);
    }
}
