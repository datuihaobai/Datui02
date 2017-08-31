using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<MeshFilter> m_Meshes = new List<MeshFilter>();
    //public static List<Terrain> m_Terrains = new List<Terrain>();

    void OnEnable()
    {
        //var m = GetComponent<MeshFilter>();
        //if (m != null)
        //{
        //    m_Meshes.Add(m);
        //}

        List<MeshFilter> ms = new List<MeshFilter>();
        GameUtility.GetComponentsInChildrenRecursive<MeshFilter>(transform, ref ms);
        for (int i = 0; i < ms.Count; i++ )
        {
            if (ms[i] != null)
                m_Meshes.Add(ms[i]);
        }

        //var t = GetComponent<Terrain>();
        //if (t != null)
        //{
        //    m_Terrains.Add(t);
        //}
    }

    void OnDisable()
    {
        //var m = GetComponent<MeshFilter>();
        //if (m != null)
        //{
        //    m_Meshes.Remove(m);
        //}

        List<MeshFilter> ms = new List<MeshFilter>();
        GameUtility.GetComponentsInChildrenRecursive<MeshFilter>(transform, ref ms);
        for (int i = 0; i < ms.Count; i++)
        {
            if (ms[i] != null)
                m_Meshes.Remove(ms[i]);
        }

        //var t = GetComponent<Terrain>();
        //if (t != null)
        //{
        //    m_Terrains.Remove(t);
        //}
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (var i = 0; i < m_Meshes.Count; ++i)
        {
            var mf = m_Meshes[i];
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = 0;
            sources.Add(s);

            //var s = new NavMeshBuildSource();
            //s.shape = NavMeshBuildSourceShape.Box;
            //s.sourceObject = m;
            //s.transform = mf.transform.localToWorldMatrix;
            //s.area = 0;
            //Collider collider = mf.GetComponent<Collider>();
            //s.component = collider;
            //s.size = new Vector3(collider.bounds.size.x / mf.transform.lossyScale.x, 
            //    collider.bounds.size.y / mf.transform.lossyScale.y,
            //    collider.bounds.size.z / mf.transform.lossyScale.z);
            //sources.Add(s);
        }

        //for (var i = 0; i < m_Terrains.Count; ++i)
        //{
        //    var t = m_Terrains[i];
        //    if (t == null) continue;

        //    var s = new NavMeshBuildSource();
        //    s.shape = NavMeshBuildSourceShape.Terrain;
        //    s.sourceObject = t.terrainData;
        //    // Terrain system only supports translation - so we pass translation only to back-end
        //    s.transform = Matrix4x4.TRS(t.transform.position, Quaternion.identity, Vector3.one);
        //    s.area = 0;
        //    sources.Add(s);
        //}
    }
}
