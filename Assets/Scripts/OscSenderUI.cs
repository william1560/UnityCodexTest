using extOSC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OscUi
{
    [DisallowMultipleComponent]
    public class OscSenderUI : MonoBehaviour
    {
        [Header("OSC Settings")]
        [SerializeField]
        private string remoteHost = "127.0.0.1";

        [SerializeField]
        private int remotePort = 9000;

        [SerializeField]
        private string messageAddress = "/unity/message";

        private OSCTransmitter transmitter;

        [Header("UI References")]
        [SerializeField]
        private InputField messageField;

        [SerializeField]
        private Button sendButton;

        private void Reset()
        {
            messageField = GetComponentInChildren<InputField>();
            sendButton = GetComponentInChildren<Button>();
        }

        private void Awake()
        {
            CreateTransmitter();
            EnsureUiReferences();
            HookupButton();
        }

        private void OnEnable()
        {
            HookupButton();
        }

        private void OnDisable()
        {
            if (sendButton != null)
            {
                sendButton.onClick.RemoveListener(SendOscMessage);
            }
        }

        private void OnValidate()
        {
            ApplyConnectionSettings();
            EnsureUiReferences();
        }

        private void CreateTransmitter()
        {
            if (transmitter == null)
            {
                transmitter = GetComponent<OSCTransmitter>();
            }

            if (transmitter == null)
            {
                transmitter = gameObject.AddComponent<OSCTransmitter>();
            }

            ApplyConnectionSettings();
        }

        private void ApplyConnectionSettings()
        {
            if (transmitter == null)
            {
                return;
            }

            transmitter.RemoteHost = remoteHost;
            transmitter.RemotePort = remotePort;
        }

        private void EnsureUiReferences()
        {
            if (messageField == null)
            {
                messageField = GetComponentInChildren<InputField>();
            }

            if (sendButton == null)
            {
                sendButton = GetComponentInChildren<Button>();
            }

            if (EventSystem.current == null)
            {
                var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystemObject.transform.SetParent(transform.root != null ? transform.root : transform, false);
            }
        }

        private void HookupButton()
        {
            if (sendButton == null)
            {
                return;
            }

            sendButton.onClick.RemoveListener(SendOscMessage);
            sendButton.onClick.AddListener(SendOscMessage);
        }

        private void SendOscMessage()
        {
            if (transmitter == null)
            {
                Debug.LogWarning("OSC transmitter is not available.");
                return;
            }

            var payload = messageField != null ? messageField.text : string.Empty;
            var message = new OSCMessage(messageAddress);
            message.AddValue(OSCValue.String(payload ?? string.Empty));

            transmitter.Send(message);

            Debug.Log($"OSC message sent to {remoteHost}:{remotePort} on '{messageAddress}' with payload '{payload}'.");
        }
    }
}
