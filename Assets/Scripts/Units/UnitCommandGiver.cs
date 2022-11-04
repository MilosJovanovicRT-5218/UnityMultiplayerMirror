using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;

    private void Start()
    {
        ///unitSelectionHandler.SelectedUnits = new List<Unit>();
        mainCamera = Camera.main;

        GameOverHandler.ClientGameOver += ClientHandleGameOver;

    }

    private void OnDestroy()
    {
        GameOverHandler.ClientGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;//ako ne pritisnemo desni klik ne zelim da mi radi nikakvu komandu
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());//cita stanje misa

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {

            if (target.hasAuthority)//ne moze da se napise hasAuthority ako nije U Targetble NetworkBehaviour
            {
                TryMove(hit.point);
                return;
            }
            TryTatget(target);
            return;
        }

        TryMove(hit.point);

    }

    private void TryMove(Vector3 point)
    {

        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {

            unit.GetPlayerMovement().CmdMove(point);

        }

    }

    private void TryTatget(Targetable target)
    {

        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {

            unit.GetTargetter().CmdSetTarget(target.gameObject);

        }

    }

    private void ClientHandleGameOver(string winnerName)
    {

        enabled = false;//ako je kraj disabe-uje

    }

}
