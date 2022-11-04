using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.EventSystems;
using System;

public class MIniMap : MonoBehaviour , IPointerDownHandler, IDragHandler
{

    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private float mapScale = 20f;//50
    [SerializeField] private float offset = -6f;

    private Transform playerCameraTransform;

    private void Update()
    {

        if (playerCameraTransform != null)
        {
            return;
        }

        if (NetworkClient.connection.identity == null)
        {
            return;
        }

        playerCameraTransform = NetworkClient.connection.identity.transform.GetComponent<RTSPlayer>().GetCameraTransform();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }

    private void MoveCamera()
    {

        Vector2 mousePos = Mouse.current.position.ReadValue();//pozicija misa na ekranu

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect,
            mousePos,
            null,
            out Vector2 localPoint
             ))
        {
            return;
        }

        Vector2 lerp = new Vector2((localPoint.x - minimapRect.rect.x) / minimapRect.rect.width, (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);

        Vector3 newCameraPos = new Vector3(
            Mathf.Lerp(-mapScale, mapScale, lerp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y));

        //Podesavanje nove pozicije
        playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, offset);
    }

}
