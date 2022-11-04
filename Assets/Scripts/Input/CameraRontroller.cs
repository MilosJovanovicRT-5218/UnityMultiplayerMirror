using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CameraRontroller : NetworkBehaviour
{

    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBoarderThickness = 10f;
    [SerializeField] private Vector2 screenXlimit = Vector2.zero;
    [SerializeField] private Vector2 screenZlimit = Vector2.zero;

    private Vector2 previousInput;

    private Controls controls;

    public override void OnStartAuthority()
    {

        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();//kotrole su = kontrole iz input sistema

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();

    }

    [ClientCallback]
    private void Update()
    {

        if (!hasAuthority || !Application.isFocused)
        {
            return;
        }

        UpdateCameraPosition();//update-uje poziciju kamere

    }

    private void UpdateCameraPosition()//update-uje poziciju kamere za mis
    {

        Vector3 pos = playerCameraTransform.position;

        if (previousInput == Vector2.zero)
        {
            //da li je mis na ivicama ekrana
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBoarderThickness)
            {
                cursorMovement.z += 1;
            }

            else if (cursorPosition.y <= screenBoarderThickness)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x  >= Screen.width - screenBoarderThickness)
            {
                cursorMovement.x += 1;
            }

            else if (cursorPosition.x <= screenBoarderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;

        }

        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.x) * speed *Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXlimit.x, screenXlimit.y);
        pos.z = Mathf.Clamp(pos.z, screenXlimit.x, screenXlimit.y);

        playerCameraTransform.position = pos; 

    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {

        previousInput = ctx.ReadValue<Vector2>();

    }

}
