using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public GameController Controller;

    private Vector3 _MousePos = Vector3.zero;
    private Vector3 _WorldPoint = Vector3.zero;
    private ConveyorItem _LastItemClicked;
    private Vector3 _MousePosOnItemClick = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _MousePos = Input.mousePosition;
        _MousePos.z = -(Camera.main.transform.position.z);
        _WorldPoint = Camera.main.ScreenToWorldPoint(_MousePos);
        _WorldPoint.z = 0f;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Controller.UpdatePauseState();
        }

        CheckCandyClickInputs();
    }

    private void CheckCandyClickInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _LastItemClicked = CheckItemClick();
            if (_LastItemClicked != null)
            {
                _MousePosOnItemClick = _MousePos;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (_LastItemClicked != null)
            {
                if (_MousePos.y > _MousePosOnItemClick.y)
                {
                    Controller.TryChangeBelt(_LastItemClicked);
                    _LastItemClicked = null;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _LastItemClicked = null;
        }
    }

    private ConveyorItem CheckItemClick()
    {
        RaycastHit2D hit = Physics2D.Raycast(_WorldPoint, Vector2.zero, 20f);
        ConveyorItem rtn = null;

        if(hit && hit.transform.GetComponent<ConveyorItem>() != null)
        {
            rtn = hit.transform.GetComponent<ConveyorItem>();
        }
        return rtn;
    }
}
