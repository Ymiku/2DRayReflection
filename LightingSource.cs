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
	private Vector3[] _verts2;
	private Vector3[] _vertsTemp;
	private int[] _triangles;
	private Vector2[] _uvs;
	private float _uvOffset;
	// Use this for initialization
	void Start () {
		if (spiltCount <= 0)
			spiltCount = 1;
		dir.Normalize ();
		_filter = GetComponent<MeshFilter> ();
		_mesh = new Mesh ();
		_mesh.bounds = new Bounds (Vector3.zero,Vector3.one*length);
		_filter.sharedMesh = _mesh;
		_verts = new Vector3[2+spiltCount*2];
		_triangles = new int[spiltCount*6];
		_uvs = new Vector2[2+spiltCount*2];
		yOffset = length / spiltCount;
		_uvOffset = 1f / spiltCount;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMesh ();
	}
	void UpdateMesh()
	{
		Vector2 worldDir = transform.TransformDirection ((Vector3)dir);
		Vector2 worldOffset = (Vector2)transform.TransformDirection (new Vector3(0f,yOffset,0f));
		int vertCount = 0;
		int uvCount = 0;
		Vector2 localOriPos = Vector2.zero;
		Vector2 worldOriPos = (Vector2)transform.position;
		RaycastHit2D hitInfo;
		for (int i = 0; i < spiltCount+1; i++) {
			hitInfo = Physics2D.Raycast (worldOriPos,worldDir,distance);
			_verts [vertCount] = new Vector3 (localOriPos.x,localOriPos.y,0f);
			if (hitInfo.collider != null) {
				_verts [vertCount + 1] = transform.InverseTransformPoint(new Vector3(hitInfo.point.x, hitInfo.point.y,transform.position.z));
			} else {
				_verts [vertCount + 1] = (Vector3)dir*distance+new Vector3(0f,localOriPos.y,0f);
			}
			_uvs [vertCount] = new Vector2 (0f,uvCount*_uvOffset);
			_uvs [vertCount+1] = new Vector2 (1f,uvCount*_uvOffset);
			worldOriPos += worldOffset;
			localOriPos += new Vector2 (0f,yOffset);
			vertCount += 2;
			uvCount++;
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
		if (_verts2 == null) {
			_verts2 = new Vector3[_verts.Length];
			for (int i = 0; i < _verts2.Length; i++) {
				_verts2 [i] = _verts [i];
			}
		} else {
			for (int i = 0; i < _verts2.Length; i++) {
				_verts2 [i] = Vector3.Lerp (_verts2 [i], _verts [i], 10f * Time.deltaTime);
			}
		}
		_mesh.vertices = _verts2;
		_mesh.triangles = _triangles;
		_mesh.uv = _uvs;
		//_filter.mesh = _mesh;
	}
}
