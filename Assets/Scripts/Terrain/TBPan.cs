using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[AddComponentMenu( "FingerGestures/Toolbox/Camera/Pan" )]
//[RequireComponent( typeof( DragRecognizer ) )]
public class TBPan : MonoBehaviour
{
    public static bool isMoving = false;

    Transform cachedTransform;

    //public float sensitivity = 1.0f;
    public float smoothSpeed = 10;
    public BoxCollider moveArea;    // the area to constrain camera movement to

    Vector3 idealPos;
    //DragGesture dragGesture;

    public delegate void PanEventHandler( TBPan source, Vector3 move );
    public event PanEventHandler OnPan;

    void Awake()
    {
        cachedTransform = this.transform;
    }

    void Start()
    {
        idealPos = cachedTransform.position;

        // sanity check
        //if( !GetComponent<DragRecognizer>() )
        //{
        //    Debug.LogWarning( "No drag recognizer found on " + this.name + ". Disabling TBPan." );
        //    enabled = false;
        //}
    }

    //void OnDrag( DragGesture gesture )
    //{
    //    dragGesture = ( gesture.State == GestureRecognitionState.Ended ) ? null : gesture;
    //}

    public void SetCameraPosition(Vector3 vPos)
    {
        vPos = ConstrainToMoveArea(vPos);
        
        //if (smoothSpeed > 0)
        //    trCameraRoot.position = Vector3.Lerp(trCameraRoot.position, vPos, Time.deltaTime * smoothSpeed);
        //else
            trCameraRoot.position = vPos;
    }

    public Transform trCameraRoot;
    public LayerMask colliderLayerMask;

    private Ray ray;
    private Vector3 mousePosStart = Vector3.zero;
    private Vector3 vCamRootPosStart = Vector3.zero;
    private Vector3 vPickOld;
    private Vector3 vPickStart;
    private bool inertiaActive;
    private float inertiaAge;
    private Vector3 inertiaSpeed;
    private bool isPointerOverToPlaceBuilding = false;
    private Transform hitTrans = null;

    void Update()
    {
        if (Input.touchCount > 1)
        {
            //Debug.Log("Input.touchCount " + Input.touchCount);
            return;
        }

        if (this.inertiaActive && (this.inertiaSpeed.magnitude > 0.01f))
        {
            this.SetCameraPosition(this.trCameraRoot.position - this.inertiaSpeed);
            this.inertiaSpeed = Vector3.Lerp(this.inertiaSpeed, Vector3.zero, this.inertiaAge);
            this.inertiaAge += Time.smoothDeltaTime;
        }
        else
        {
            this.inertiaActive = false;
        }

        RaycastHit hit;
        Vector3 mousePosition = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            if (ClickIsOverUI.instance.IsPointerOverUIObject(Input.mousePosition))
                return;
            if (TerrainManager.instance.IsPointerOverToPlaceBuilding())
            {
                isPointerOverToPlaceBuilding = true;
                return;
            }
            else
                isPointerOverToPlaceBuilding = false;
            //Debug.Log("GetMouseButtonDown mousePosition " + mousePosition);
            mousePosStart = mousePosition;
            vCamRootPosStart = trCameraRoot.position;
            ray = Camera.main.ScreenPointToRay(mousePosition);
            Physics.Raycast(ray, out hit, Mathf.Infinity, colliderLayerMask);
            hitTrans = hit.transform;
            if (hitTrans == null)
                return;
            //Debug.Log("GetMouseButtonDown hit.point " + hit.point);
            vPickStart = hit.point - trCameraRoot.position;
            vPickOld = vPickStart;
            inertiaActive = false;
            inertiaAge = 0f;
            inertiaSpeed = Vector3.zero;
        }
        else if (Input.GetMouseButton(0))
        {
            if (ClickIsOverUI.instance.IsPointerOverUIObject(Input.mousePosition))
                return;
            if (isPointerOverToPlaceBuilding)
            {
                isMoving = false;
                return;
            }
            if (hitTrans == null)
                return;

            if (Vector3.Distance(mousePosition, mousePosStart) > 5f)
            {
                ray = Camera.main.ScreenPointToRay(mousePosition);
                Physics.Raycast(ray, out hit, Mathf.Infinity, colliderLayerMask);
                //Debug.Log("GetMouseButton hit.point " + hit.point);
                Vector3 vPickCurrent = hit.point - trCameraRoot.position;
                inertiaSpeed = (Vector3)((0.3f * inertiaSpeed) + (0.7f * (vPickCurrent - vPickOld)));
                Vector3 vCameraPanDir = vPickCurrent - vPickStart;
                SetCameraPosition(vCamRootPosStart - vCameraPanDir);
                vPickOld = vPickCurrent;
                isMoving = true;
            }
            else
                isMoving = false;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (ClickIsOverUI.instance.IsPointerOverUIObject(Input.mousePosition))
                return;
            if (isPointerOverToPlaceBuilding)
                return;
            if (hitTrans == null)
                return;
            if (inertiaSpeed.magnitude > 0.01f)
                inertiaActive = true;
        }
    }

    //void LateUpdate()
    //{
    //    if (dragGesture != null)
    //    {
    //        float ma = dragGesture.DeltaMove.SqrMagnitude();
    //        Debug.Log("ma " + dragGesture.DeltaMove);
    //        if (dragGesture.DeltaMove.SqrMagnitude() > 0)
    //        {
    //            isMoving = true;
    //            Vector2 screenSpaceMove = sensitivity * dragGesture.DeltaMove;
    //            Vector3 worldSpaceMove = screenSpaceMove.x * cachedTransform.right + screenSpaceMove.y * cachedTransform.up;
    //            idealPos -= worldSpaceMove;

    //            if (OnPan != null)
    //                OnPan(this, worldSpaceMove);
    //        }
    //    }
    //    else
    //        isMoving = false;

    //    idealPos = ConstrainToMoveArea(idealPos);

    //    if (smoothSpeed > 0)
    //        cachedTransform.position = Vector3.LerpUnclamped(cachedTransform.position, idealPos, Time.deltaTime * smoothSpeed);
    //    else
    //        cachedTransform.position = idealPos;
    //}

    // project point on panning plane
    public Vector3 ConstrainToPanningPlane( Vector3 p )
    {
        Vector3 lp = cachedTransform.InverseTransformPoint( p );
        lp.z = 0;
        return cachedTransform.TransformPoint( lp );
    }

    public void TeleportTo( Vector3 worldPos )
    {
        cachedTransform.position = idealPos = ConstrainToPanningPlane( worldPos );
    }

    public void FlyTo( Vector3 worldPos )
    {
        idealPos = ConstrainToPanningPlane( worldPos );
    }

    public Vector3 ConstrainToMoveArea( Vector3 p )
    {
        if( moveArea )
        {
            Vector3 min = moveArea.bounds.min;
            Vector3 max = moveArea.bounds.max;

            p.x = Mathf.Clamp( p.x, min.x, max.x );
            p.y = Mathf.Clamp( p.y, min.y, max.y );
            p.z = Mathf.Clamp( p.z, min.z, max.z );
        }

        return p;
    }
}
