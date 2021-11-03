using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// Small modification of the classic XRGrabInteractable that will keep the position and rotation offset between the
/// grabbed object and the controller instead of snapping the object to the controller. Better for UX and the illusion
/// of holding the thing (see Tomato Presence : https://owlchemylabs.com/tomatopresence/)
/// </summary>
public class SCR_XROffsetGrabbable : XRGrabInteractable
    {
    class SavedTransform
    {
        public Vector3 OriginalPosition;
        public Quaternion OriginalRotation;
    }
    private bool grabedOnce;
    public Transform follow;
    private Vector3 posStart;
    public Transform followRotation;
    Dictionary<XRBaseInteractor, SavedTransform> m_SavedTransforms = new Dictionary<XRBaseInteractor, SavedTransform>();

    Rigidbody m_Rb;
    private Vector3 startParentPosition;
    private Quaternion startParentRotationQ;
    private Vector3 startChildPosition;
    private Quaternion startChildRotationQ;
    private Matrix4x4 parentMatrix;

    protected override void Awake()
    {
        base.Awake();

        //the base class already grab it but don't expose it so have to grab it again
        m_Rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (!m_Rb)
            m_Rb = GetComponent<Rigidbody>();
        if (follow)
        {
            m_Rb.constraints = RigidbodyConstraints.FreezeAll;

            startParentPosition = follow.position;
            startParentRotationQ = follow.rotation;

            startChildPosition = transform.position;
            startChildRotationQ = transform.rotation;
            //founded by testing
            startChildPosition = DivideVectors(Quaternion.Inverse(follow.rotation) * (startChildPosition - startParentPosition), follow.lossyScale);
        }
    }
    Vector3 DivideVectors(Vector3 num, Vector3 den)
    {

        return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);

    }
    private void Update()
    {
        if(!grabedOnce && follow != null)
        {
           //simulate child effect
            parentMatrix = Matrix4x4.TRS(follow.position, follow.rotation, follow.lossyScale);

            transform.position = parentMatrix.MultiplyPoint3x4(startChildPosition);

            transform.rotation = (follow.rotation * Quaternion.Inverse(startParentRotationQ)) * startChildRotationQ;
        }
    }
    protected override void OnSelectEntering(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            SavedTransform savedTransform = new SavedTransform();

            savedTransform.OriginalPosition = interactor.attachTransform.localPosition;
            savedTransform.OriginalRotation = interactor.attachTransform.localRotation;

            m_SavedTransforms[interactor] = savedTransform;


            bool haveAttach = attachTransform != null;

            interactor.attachTransform.position = haveAttach ? attachTransform.position : m_Rb.worldCenterOfMass;
            interactor.attachTransform.rotation = haveAttach ? attachTransform.rotation : m_Rb.rotation;
        }

        base.OnSelectEntering(interactor);
    }
  
    protected override void Grab()
    {
        if (!grabedOnce)
        {
            grabedOnce = true;
            m_Rb.constraints = RigidbodyConstraints.None;

        }
        base.Grab();
    }
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
        if (interactor is XRDirectInteractor)
        {
            SavedTransform savedTransform = null;
            if (m_SavedTransforms.TryGetValue(interactor, out savedTransform))
            {
                interactor.attachTransform.localPosition = savedTransform.OriginalPosition;
                interactor.attachTransform.localRotation = savedTransform.OriginalRotation;

                m_SavedTransforms.Remove(interactor);
            }
        }

        base.OnSelectExiting(interactor);
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        int interactorLayerMask = 1 << interactor.gameObject.layer;
        return base.IsSelectableBy(interactor) && (interactionLayerMask.value & interactorLayerMask) != 0;
    }
}
