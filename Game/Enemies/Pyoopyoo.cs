using UnityEngine;

namespace ShootAR.Enemies
{
	/// <summary>
	/// Long-Ranged class of Enemy
	/// </summary>
	public abstract class Pyoopyoo : Enemy
	{
		/// <summary>
		/// 총알이 발사될 위치
		/// </summary>
		[SerializeField] protected Transform bulletSpawnPoint; // 총알이 발사될 위치를 나타내는 Transform입니다. 이는 SerializeField 어트리뷰트를 사용하여 Unity 에디터에서 설정할 수 있습니다.

		/// <summary>
		/// 플레이어를 공격하기 위해 <see cref="EnemyBullet"/>을 생성합니다.
		/// </summary>
		protected abstract void Shoot(); // 플레이어를 공격하기 위해 총알을 발사하는 메서드입니다. 이는 추상 메서드로, Pyoopyoo 클래스를 상속하는 클래스에서 구현해야 합니다.
	}
}
