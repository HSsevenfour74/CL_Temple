using UnityEngine;

namespace LineCustomization
{
	public class RotateHead : MonoBehaviour
	{
		public float rotateSpeed = 1f;

		private Transform _trans;

		private Vector3 lastPos;

		private Vector3 delta;

		private Transform trans
		{
			get
			{
				if (_trans == null)
				{
					_trans = base.transform;
				}
				return _trans;
			}
		}

		public void Reset()
		{
			lastPos = trans.position;
			trans.localRotation = Quaternion.identity;
		}

		private void OnEnable()
		{
			lastPos = trans.position;
		}

		private void Update()
		{
			delta = trans.position - lastPos;
			lastPos = trans.position;
			delta *= Time.smoothDeltaTime * rotateSpeed;
			trans.Rotate(delta.z, 0f, 0f - delta.x, Space.World);
		}
	}
}
