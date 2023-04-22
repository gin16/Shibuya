using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameOption : NetworkBehaviour
{
    [SerializeField] int[] values;
    [SerializeField] TMPro.TextMeshProUGUI valueText;


    [Networked(OnChanged = nameof(OnChangedIndex))]
    public int Index { get; private set; } = 0;
    public int Value { get { return values[Index]; } }


    bool busy;

    /// <summary>
    /// For displaying in inspector view
    /// </summary>
    [Tooltip("For displaying in inspector view")] [SerializeField] int AuthorityPlayerId;

    public override void Spawned() {
        valueText.text = "" + Value;
    }

    public override void FixedUpdateNetwork() {
        if (!busy && Object.HasStateAuthority) {
            Object.ReleaseStateAuthority();
        }
        AuthorityPlayerId = Object.StateAuthority.PlayerId;
    }

    public void OnClickAddIndex(int value) {
        if (busy) return;
        if (Game.Main == null || Game.Main.Phase != GamePhase.Top) return;
        StartCoroutine(AddIndex(value));
    }

    IEnumerator AddIndex(int value) {
        Debug.Log($"{name} Add Index {value} BEGIN");
        busy = true;
        Object.RequestStateAuthority();
        for (float t = 0; !Object.HasStateAuthority; t += Time.deltaTime) {
            if (t > 1) {
                Debug.Log($"{name} Add Index {value} TIME OUT");
                busy = false;
                yield break;
            }
            yield return null;
        }
        Index = Mathf.Clamp(Index + value, 0, values.Length - 1);
        busy = false;
        Object.ReleaseStateAuthority();
        Debug.Log($"{name} Add Index {value} END");
    }

    public static void OnChangedIndex(Changed<GameOption> changed) {
        changed.Behaviour.valueText.text = "" + changed.Behaviour.Value;
    }
}
