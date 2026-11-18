using System.Collections;
using UnityEngine;

public sealed class DungeonEntranceDoor : MonoBehaviour
{
    [SerializeField] private QuestDefinition quest;
    [SerializeField] private Transform leaf;
    [SerializeField] private Collider2D blocker;
    [SerializeField, Min(.05f)] private float openingDuration = .6f;

    private Vector3 closedScale;
    private Coroutine opening;

    public bool IsOpen { get; private set; }

    private void Awake() => closedScale = leaf.localScale;

    private void OnEnable()
    {
        QuestJournal.Changed += OnQuestChanged;
        SetOpen(QuestJournal.GetState(quest) == QuestState.Active);
    }

    private void OnDisable()
    {
        QuestJournal.Changed -= OnQuestChanged;
        CancelOpening();
    }

    private void OnQuestChanged(QuestDefinition changed)
    {
        if (changed != quest)
            return;
        if (QuestJournal.GetState(quest) != QuestState.Active)
        {
            CancelOpening();
            SetOpen(false);
        }
        else if (!IsOpen && opening == null)
            opening = StartCoroutine(Open());
    }

    private IEnumerator Open()
    {
        for (float elapsed = 0f; elapsed < openingDuration; elapsed += Time.unscaledDeltaTime)
        {
            SetLeafWidth(Mathf.SmoothStep(1f, .12f, elapsed / openingDuration));
            yield return null;
        }
        opening = null;
        SetOpen(true);
    }

    private void SetOpen(bool value)
    {
        SetLeafWidth(value ? .12f : 1f);
        blocker.enabled = !value;
        IsOpen = value;
    }

    private void SetLeafWidth(float width) =>
        leaf.localScale = new Vector3(closedScale.x * width, closedScale.y, closedScale.z);

    private void CancelOpening()
    {
        if (opening != null)
            StopCoroutine(opening);
        opening = null;
    }
}
