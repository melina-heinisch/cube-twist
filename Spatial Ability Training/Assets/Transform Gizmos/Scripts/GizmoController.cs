using UnityEngine;

namespace TransformGizmos
{
    public class GizmoController : MonoBehaviour
    {
        [SerializeField] public Rotation m_rotation;
        [SerializeField] GameObject m_rotationAppendix;


        [SerializeField] Material m_clickedMaterial;
        [SerializeField] Material m_transparentMaterial;
        [SerializeField] GameObject m_objectWithMeshes;

        [Header("Adjustable Variables")]
        [SerializeField] public GameObject m_targetObject;
        [SerializeField] float m_gizmoSize = 1;

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (!(m_targetObject is null))
            { 
                transform.SetPositionAndRotation(m_targetObject.transform.position, m_targetObject.transform.rotation);
                m_objectWithMeshes.transform.position = m_targetObject.transform.position;
                m_rotation.SetGizmoSize(m_gizmoSize);  
            }
            
        }

        public void Init()
        {
            m_rotation.Initialization(m_targetObject, m_clickedMaterial, m_transparentMaterial, m_objectWithMeshes, m_rotationAppendix);
            transform.SetPositionAndRotation(m_targetObject.transform.position, m_targetObject.transform.rotation);
            transform.localScale = m_targetObject.transform.localScale;
        }
    }
}
