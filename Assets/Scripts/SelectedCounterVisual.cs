using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject selectedCounterVisual;

    private void Start() {
        Player.Instance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
    }

    private void Player_OnSelectedCounterChange(object sender, Player.OnSelectedCounterChangeEventArgs e) {
        if (e.selectedCounter == clearCounter) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        selectedCounterVisual.SetActive(true);
    }

    private void Hide() {
        selectedCounterVisual.SetActive(false);
    }
}
