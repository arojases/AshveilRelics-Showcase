using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CircleCollider2D))]
public sealed class ScenePortal : MonoBehaviour
{
    [SerializeField] private string destinationScenePath;
    [SerializeField] private string destinationSpawnId;
    [SerializeField] private string promptLabel;
    [SerializeField] private QuestDefinition requiredActiveQuest;
    [SerializeField, TextArea] private string blockedMessage;
    [SerializeField, TextArea] private string completedMessage;
    [SerializeField] private DungeonEntranceDoor entranceDoor;
    [SerializeField] private string closedPromptLabel;
    [SerializeField, Min(0f)] private float promptHeight = .95f;

    private CircleCollider2D interactionArea;

    public string DestinationScenePath => destinationScenePath;
    public string DestinationSpawnId => destinationSpawnId;
    public float PromptHeight => promptHeight;
    public string PromptLabel => entranceDoor != null && !entranceDoor.IsOpen ? closedPromptLabel : promptLabel;
    public string BlockedMessage
    {
        get
        {
            var state = QuestJournal.GetState(requiredActiveQuest);
            if (state == QuestState.Completed && !string.IsNullOrWhiteSpace(completedMessage))
                return completedMessage;
            if (state == QuestState.Active && entranceDoor != null && !entranceDoor.IsOpen)
                return "La puerta se está abriendo.";
            return blockedMessage;
        }
    }
    public bool CanTravel => isActiveAndEnabled && (requiredActiveQuest == null ||
        QuestJournal.GetState(requiredActiveQuest) == QuestState.Active) &&
        (entranceDoor == null || entranceDoor.IsOpen);

    private void Awake() => interactionArea = GetComponent<CircleCollider2D>();

    public bool CanInteract(Vector2 position) =>
        isActiveAndEnabled && interactionArea.enabled && interactionArea.OverlapPoint(position);
}
