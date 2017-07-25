using UnityEngine;
using System.Collections;

public class LightingSource : MonoBehaviour {
	public float length = 1f;
	public float distance = 10f;
	public Vector2 dir;
	public int spiltCount = 5;
	private float yOffset;
	private MeshFilter _filter;
	private Mesh _mesh;
	private Vector3[] _verts;
	private int[] _triangles;
	// Use this for initialization
	void Start () {
		if (spiltCount <= 0)
			spiltCount = 1;
		dir.Normalize ();
		_filter = GetComponent<MeshFilter> ();
		_mesh = new Mesh ();
		_filter.mesh = _mesh;
		_verts = new Vector3[2+spiltCount*2];
		_triangles = new int[spiltCount*6];
		yOffset = length / spiltCount;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMesh ();
	}
	void UpdateMesh()
	{
		Vector2 worldDir = transform.TransformDirection ((Vector3)dir);
		int vertCount = 0;
		Vector2 localOriPos = Vector2.zero;
		Vector2 worldOriPos = (Vector2)transform.position;
		RaycastHit2D hitInfo;
		for (int i = 0; i < spiltCount+1; i++) {
			hitInfo = Physics2D.Raycast (worldOriPos,worldDir,distance);
			_verts [vertCount] = new Vector3 (localOriPos.x,localOriPos.y,transform.position.z);
			if (hitInfo.collider != null) {
				_verts [vertCount + 1] = transform.InverseTransformPoint(new Vector3(hitInfo.point.x, hitInfo.point.y,transform.position.z));
			} else {
				_verts [vertCount + 1] = (Vector3)dir*distance+new Vector3(0f,localOriPos.y,0f);
			}
			worldOriPos += new Vector2 (0f,yOffset);
			localOriPos += new Vector2 (0f,yOffset);
			vertCount += 2;
		}
		int trianglesCount = 0;
		vertCount = 0;
		for (int i = 0; i < spiltCount; i++) {
			_triangles[trianglesCount] = vertCount;
			_triangles[trianglesCount+1] = vertCount+2;
			_triangles[trianglesCount+2] = vertCount+1;
			_triangles[trianglesCount+3] = vertCount+1;
			_triangles[trianglesCount+4] = vertCount+2;
			_triangles[trianglesCount+5] = vertCount+3;
			vertCount += 2;
			trianglesCount += 6;
		}
		_mesh.vertices = _verts;
		_mesh.triangles = _triangles;
		_filter.mesh = _mesh;
	}
}
