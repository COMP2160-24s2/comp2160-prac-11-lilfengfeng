/**
 * A singleton class to allow point-and-click movement of the marble.
 * 
 * It publishes a TargetSelected event which is invoked whenever a new target is selected.
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;
using UnityEngine.InputSystem;

// note this has to run earlier than other classes which subscribe to the TargetSelected event
[DefaultExecutionOrder(-100)]
public class UIManager : MonoBehaviour
{
#region UI Elements
    [SerializeField] private Transform crosshair;
    [SerializeField] private Transform target;
#endregion 

#region Singleton
    static private UIManager instance;
    static public UIManager Instance
    {
        get { return instance; }
    }
#endregion 

#region Actions
    private Actions actions;
    private InputAction mouseAction;
    private InputAction deltaAction;
    private InputAction selectAction;
#endregion

#region Events
    public delegate void TargetSelectedEventHandler(Vector3 worldPosition);
    public event TargetSelectedEventHandler TargetSelected;
#endregion

#region Init & Destroy
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one UIManager in the scene.");
        }

        instance = this;

        actions = new Actions();
        mouseAction = actions.mouse.position;
        deltaAction = actions.mouse.delta;
        selectAction = actions.mouse.select;

        Cursor.visible = false;
        target.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        actions.mouse.Enable();
    }

    void OnDisable()
    {
        actions.mouse.Disable();
    }
#endregion Init

#region Update
    void Update()
    {
        MoveCrosshair();
        SelectTarget();
    }

    private void MoveCrosshair() 
    {
                Camera cam = Camera.main;

        Vector2 mousePos = mouseAction.ReadValue<Vector2>();

    // Adjust Z based on your camera type
    float zDistance = 10f; // Adjust this as necessary
    Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, zDistance);

    // Convert from screen coordinates to world coordinates
    Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

    // Update crosshair position
    // crosshair.position = worldPos;
    crosshair.position = new Vector3(worldPos.x, 0, worldPos.z); // Keep original Z for the crosshair

    // Debugging output to check position
    Debug.Log($"Crosshair World Position: {crosshair.position}");

        // Vector2 mousePos = mouseAction.ReadValue<Vector2>();
        // Vector3 pos = new Vector3(mousePos.x, mousePos.y, 10);

        // Vector3 worldPos = cam.WorldToScreenPoint(pos);

        // crosshair.position = new Vector3(worldPos.x, 0.1f, worldPos.z);
        // Debug.Log(cam.WorldToScreenPoint(crosshair.position));



        // Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, -10);
        

        // // FIXME: Move the crosshair position to the mouse position (in world coordinates)
        // crosshair.position = cam.WorldToScreenPoint(screenPos);
    }

    private void SelectTarget()
    {
        if (selectAction.WasPerformedThisFrame())
        {
            // set the target position and invoke 
            target.gameObject.SetActive(true);
            target.position = crosshair.position;     
            TargetSelected?.Invoke(target.position);       
        }
    }

#endregion Update

}
