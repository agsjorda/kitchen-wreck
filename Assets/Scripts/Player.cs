using System;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangeEventArgs> OnSelectedCounterChange;
    public class OnSelectedCounterChangeEventArgs : EventArgs {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;



    private bool isWalking;
    private Vector2 inputVector;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance in the scene");
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InterAct();
        }
    }

    // Update is called once per frame
    private void Update() {
        HandleMovment();
        HandleInteractions();
    }


    public bool IsWalking() {
        return isWalking;
    }
    private void HandleInteractions() {
        inputVector = gameInput.GetMovementInputNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                //clearCounter.InterAct();
                if (clearCounter != selectedCounter) {
                    SetSelectedCounter(clearCounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovment() {

        inputVector = gameInput.GetMovementInputNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        // Draw the raycast in the Scene view
        Debug.DrawRay(transform.position, moveDir * playerRadius, Color.red);

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            // if can't move forward, try to move to the right or left
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove) {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;


        // make the player look at the direction it is moving and make transition smooth
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 10);

    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChange?.Invoke(this, new OnSelectedCounterChangeEventArgs { selectedCounter = selectedCounter });
    }
}
