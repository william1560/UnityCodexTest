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
        private InputField messageField;

        private void Awake()
        {
            CreateTransmitter();
            CreateInterface();
        }

        private void OnValidate()
        {
            ApplyConnectionSettings();
        }

        private void CreateTransmitter()
        {
            if (transmitter != null)
            {
                return;
            }

            var transmitterObject = new GameObject("OSC Transmitter");
            transmitterObject.transform.SetParent(transform, false);

            transmitter = transmitterObject.AddComponent<OSCTransmitter>();
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

        private void CreateInterface()
        {
            if (messageField != null)
            {
                return;
            }

            var canvasObject = new GameObject("OscSenderCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;

            var panelObject = new GameObject("Panel", typeof(Image));
            panelObject.transform.SetParent(canvasObject.transform, false);

            var panelRect = panelObject.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(420f, 200f);
            panelRect.anchoredPosition = Vector2.zero;

            var panelImage = panelObject.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.4f);

            var font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var inputObject = new GameObject("MessageInput", typeof(Image), typeof(InputField));
            inputObject.transform.SetParent(panelObject.transform, false);

            var inputRect = inputObject.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0.5f, 0.5f);
            inputRect.anchorMax = new Vector2(0.5f, 0.5f);
            inputRect.pivot = new Vector2(0.5f, 0.5f);
            inputRect.sizeDelta = new Vector2(360f, 48f);
            inputRect.anchoredPosition = new Vector2(0f, 35f);

            var inputImage = inputObject.GetComponent<Image>();
            inputImage.color = Color.white;

            var placeholderObject = new GameObject("Placeholder", typeof(Text));
            placeholderObject.transform.SetParent(inputObject.transform, false);

            var placeholderRect = placeholderObject.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10f, 6f);
            placeholderRect.offsetMax = new Vector2(-10f, -6f);

            var placeholderText = placeholderObject.GetComponent<Text>();
            placeholderText.font = font;
            placeholderText.text = "Enter OSC message";
            placeholderText.alignment = TextAnchor.MiddleLeft;
            placeholderText.color = new Color(0.6f, 0.6f, 0.6f, 0.75f);
            placeholderText.supportRichText = false;

            var textObject = new GameObject("Text", typeof(Text));
            textObject.transform.SetParent(inputObject.transform, false);

            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10f, 6f);
            textRect.offsetMax = new Vector2(-10f, -6f);

            var textComponent = textObject.GetComponent<Text>();
            textComponent.font = font;
            textComponent.text = string.Empty;
            textComponent.alignment = TextAnchor.MiddleLeft;
            textComponent.color = Color.black;
            textComponent.supportRichText = false;

            var inputField = inputObject.GetComponent<InputField>();
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderText;
            inputField.targetGraphic = inputImage;
            inputField.lineType = InputField.LineType.SingleLine;

            messageField = inputField;

            var buttonObject = new GameObject("SendButton", typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(panelObject.transform, false);

            var buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.sizeDelta = new Vector2(200f, 48f);
            buttonRect.anchoredPosition = new Vector2(0f, -50f);

            var buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.24f, 0.51f, 0.96f, 1f);

            var buttonTextObject = new GameObject("Text", typeof(Text));
            buttonTextObject.transform.SetParent(buttonObject.transform, false);

            var buttonTextRect = buttonTextObject.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;

            var buttonText = buttonTextObject.GetComponent<Text>();
            buttonText.font = font;
            buttonText.text = "Send OSC";
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            buttonText.supportRichText = false;

            var sendButton = buttonObject.GetComponent<Button>();
            sendButton.onClick.AddListener(SendOscMessage);

            if (EventSystem.current == null)
            {
                var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystemObject.transform.SetParent(transform, false);
            }
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
