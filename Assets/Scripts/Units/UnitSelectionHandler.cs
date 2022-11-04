using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSelectionHandler : MonoBehaviour
{

    [SerializeField] private RectTransform unitSelectionArea = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Vector2 startPosition;

    private RTSPlayer player;

    private Camera mainCamera;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    // Start is called before the first frame update
    private void Start()
    {

        mainCamera = Camera.main;

        Unit.AuthorityOnUnitDeSpawned += AuthorityOnUnitDeSpawned;
        GameOverHandler.ClientGameOver += ClientHandleGameOver;

        //player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDeSpawned -= AuthorityOnUnitDeSpawned;
        GameOverHandler.ClientGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {

        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            //end selection area
            ClearSelectionArea();
        }

        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {

        if (!Keyboard.current.leftShiftKey.isPressed)//Ako nije pritisnut shift
        {
            //Start selection erea
            foreach (Unit selectedUnits in SelectedUnits)
            {
                selectedUnits.DeSelect();//bez ovoga nece da deselektuje tenkice,uvek bi bio aktivan selektor
            }

            SelectedUnits.Clear();//Ocisti listu
//bez ovoga ne bi krenuo drugi selektovani igrac jer nije ocistio odnosno obnovio lisy slektovanih units-a
        }

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();//pocinje kad je pritisnut levi klik//proveri vidi pogledaj

        UpdateSelectionArea();

    }
    
    private void UpdateSelectionArea()
    {

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {
          
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)//ako nije kliknut mis i prestao sam da prevlacim(drag) misem
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
            {
                return;
            }

            if (!unit.hasAuthority)
            {
                return;
            }

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnits in SelectedUnits)
            {
                selectedUnits.Select();
            }

            return;

        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.GetMyUnits())
        {

            if (SelectedUnits.Contains(unit))
            {
                continue;
            }

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }

    }

    private void AuthorityOnUnitDeSpawned(Unit unit)
    {

        SelectedUnits.Remove(unit);

    }

    private void ClientHandleGameOver(string winnerName)
    {

        enabled = false;

    }

}
