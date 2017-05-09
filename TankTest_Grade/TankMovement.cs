using UnityEngine;

public class TankMovement : MonoBehaviour
{
	#region -----Public Field-----
	public delegate void MoveStatusDelegate(int num);
	public static int status;              // 0:idle 1:turning 2:moving 3:collide 4:turned 5:moved
	public MoveStatusDelegate moveStatusDelegate; //탱크의 이동 상태 call back
	#endregion
	#region -------Private Field-------
	private float speed = 12f;                 // How fast the tank moves forward and back.
	private float turnSpeed = 180f;            // How fast the tank turns in degrees per second.
	private Vector3 point;
	private Rigidbody rigidbody;              // Reference used to move the tank.
	private bool turning;                          //true 일때 turn 가능
	private bool moving;                          //true 일때 move 가능
	public static float timer = 0f;
	#endregion
	
	#region -----Private Method-----
	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		turning = false;
		moving = false;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		Turn();
		Move();
		timer += Time.deltaTime;
	}

	//벽과 충돌시 event
	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "wall")
		{
			status = 3;
			moving = false;
			turning = false;
			moveStatusDelegate(status);
		}
	}
	#endregion

	#region -------Public Method-------
	public void MoveTo(Vector3 p) //Player.cs 에서 호출 직접적으로 Tank Object 이동
	{
		timer = 0f;
		point = p;
		turning = false;
		moving = true;
		status = 2;
		moveStatusDelegate(status);
	}

	public void TurnTo(Vector3 p)
	{
		timer = 0f;
		point = p;
		turning = true;
		moving = false;
		status = 1;
		moveStatusDelegate(status);
	}

	//목표 지점까지 전진
	public void Move()
	{
		if (!(moving && !turning))
			return;

		if (Vector3.Distance(transform.position, point) > 0.5f && timer < 10f)
		{
			Vector3 movement = transform.forward * speed * Time.deltaTime;
			rigidbody.MovePosition(rigidbody.position + movement);
		}
		else
		{
			moving = false;
			status = 5;
			moveStatusDelegate(status);
		}
	}

	//목표 지점을 바라보게 회전
	public void Turn()
	{
		if (!(turning && !moving))
			return;

		Vector3 vec = point - transform.position;
		vec.Normalize();
		Quaternion targetRot = Quaternion.LookRotation(vec);

		Vector3 forwardA = transform.rotation * Vector3.forward;
		Vector3 forwardB = targetRot * Vector3.forward;

		float angleA = Mathf.Atan2(forwardA.x, forwardB.z);
		float angleB = Mathf.Atan2(forwardB.x, forwardA.z);

		// Quaternion 값을 변환하여 계산, 부동소수점 오차때문에 범위를 지정
		if (!(-0.000001 < Mathf.DeltaAngle(angleA, angleB) && Mathf.DeltaAngle(angleA, angleB) < 0.000001) && timer < 10f)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
		}
		else
		{
			turning = false;
			status = 4;
			moveStatusDelegate(status);
		}
	}
	#endregion
}
