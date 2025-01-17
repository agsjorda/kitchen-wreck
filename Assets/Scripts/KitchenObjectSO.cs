using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObjectsSO", menuName = "Scriptable Objects/KitchenObjectsSO")]
public class KitchenObjectSO : ScriptableObject {

    public Transform prefab;
    public Sprite sprite;
    public string objectName;

}
