using System.Collections;
using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;
using ShootAR.Enemies;

namespace ShootAR
{
	public class Spawner : MonoBehaviour
	{
		private static XmlReader xmlPattern;

		[SerializeField] private Type objectToSpawn;
		public Type ObjectToSpawn {
			get { return objectToSpawn; }
			private set { objectToSpawn = value; }
		}
		///스폰 속도를 설정하거나 가져옵니다.
		public float SpawnRate { get; set; }
		///초기 딜레이를 가져옵니다.
		public float InitialDelay { get; private set; }
		[SerializeField] private float maxDistanceToSpawn, minDistanceToSpawn;
		///최대 스폰 거리를 설정하거나 가져옵니다.
		public float MaxDistanceToSpawn {
			get { return maxDistanceToSpawn; }
			private set { maxDistanceToSpawn = value; }
		}
		///최소 스폰거리를 설정하거나 가져옵니다.
		public float MinDistanceToSpawn {
			get { return minDistanceToSpawn; }
			private set { minDistanceToSpawn = value; }
		}
		public int SpawnLimit { get; private set; }
		public int SpawnCount { get; private set; }
		public bool IsSpawning { get; private set; } = false;

		
		private static GameState gameState;
		

		private Coroutine lastSpawnCall;

		private void Awake() {
			// Awake 메서드가 호출될 때, SpawnLimit이 0이면 -1로 설정합니다.
			if (SpawnLimit == 0) SpawnLimit = -1;
		}

		public static Spawner Create(
				Type objectToSpawn = null, int spawnLimit = 30,
				float initialDelay = 3f, float spawnRate = 60f,
				float maxDistanceToSpawn = 100f,
				float minDistanceToSpawn = 80f,
				GameState gameState = null) {
			// 새로운 GameObject를 생성하고 Spawner 컴포넌트를 추가합니다.
			var o = new GameObject(nameof(Spawner)).AddComponent<Spawner>();
			// 전달된 매개변수들로 Spawner의 속성을 초기화합니다.
			o.ObjectToSpawn = objectToSpawn;
			o.SpawnLimit = spawnLimit;
			o.SpawnRate = spawnRate;
			o.MaxDistanceToSpawn = maxDistanceToSpawn;
			o.MinDistanceToSpawn = minDistanceToSpawn;
			Spawner.gameState = gameState;
			// Spawner의 OnEnable 메서드를 호출합니다.
			o.OnEnable();
			// 생성된 Spawner 객체를 반환합니다.
			return o;
		}

		public struct SpawnConfig
		{
			// 읽기 전용 필드로 구성된 구조체인 SpawnConfig 선언
			public readonly Type type; //스폰할 오브젝트의 타입
			public readonly int limit; // 스폰 제한 수
			public readonly float rate, delay, maxDistance, 
			minDistance; //스폰 속도, 초기 딜레이, 최대 스폰 거리, 최소 스폰 거리

			// 생성자 매서드로 구조체 초기화
			public SpawnConfig(
					Type type, int limit, float rate, float delay,
					float minDistance, float maxDistance) {
				// 생성자에서 각 필드를 전달된 값으로 초기화
				this.type = type;
				this.limit = limit;
				this.rate = rate;
				this.delay = delay;
				this.maxDistance = maxDistance;
				this.minDistance = minDistance;
			}
		}

		public void Configure(
			Type type, int limit,
			float rate, float delay,
			float minDistance, float maxDistance
		) {
			// 메서드를 통해 Spawner의 속성들을 설정하는 메서드입니다.

            // Configure 메서드의 매개변수로 전달받은 값을 각 속성에 할당합니다.
			ObjectToSpawn = type;            // 스폰할 오브젝트의 타입
			SpawnLimit = limit;               // 스폰 제한 수
			SpawnRate = rate;                 // 스폰 속도
			InitialDelay = delay;             // 초기 딜레이
			MinDistanceToSpawn = minDistance; // 최소 스폰 거리
			MaxDistanceToSpawn = maxDistance; // 최대 스폰 거리
		}

		public void Configure(SpawnConfig config) =>
			Configure(
				config.type, config.limit, config.rate,
				config.delay, config.minDistance, config.maxDistance
			);

		private void Start() {
			// Start 메서드는 MonoBehaviour의 생명주기 중 하나인 초기화 단계에서 호출됩니다.

            // gameState가 null이면 FindObjectOfType를 통해 GameState 객체를 찾아 할당합니다.
			if (gameState is null)
				gameState = FindObjectOfType<GameState>();
		}

		private void OnEnable() {
			// OnEnable 메서드는 MonoBehaviour가 활성화될 때 호출됩니다.

            // gameState가 null이 아니면 해당 게임 상태 이벤트에 대한 처리를 추가합니다.
			if (gameState != null) {
				gameState.OnGameOver += StopSpawning;
				gameState.OnRoundWon += StopSpawning;
			}
		}

		private void OnDisable() {
			// OnDisable 메서드는 MonoBehaviour가 비활성화될 때 호출됩니다.

            // gameState가 null이 아니면 해당 게임 상태 이벤트에 대한 처리를 제거합니다.
			if (gameState != null) {
				gameState.OnGameOver -= StopSpawning;
				gameState.OnRoundWon -= StopSpawning;
			}
		}

		
private IEnumerator Spawn()
{
    yield return new WaitForSeconds(InitialDelay);
    while (IsSpawning)
    {
        yield return new WaitForSeconds(SpawnRate);

        if (!IsSpawning) break;
        if (SpawnCount >= Spawnable.GLOBAL_SPAWN_LIMIT) continue;

        float r = UnityEngine.Random.Range(minDistanceToSpawn, maxDistanceToSpawn);
				float theta = UnityEngine.Random.Range(0f, Mathf.PI/4);
				float fi = UnityEngine.Random.Range(0f, 2 * Mathf.PI/4);
				//구형 좌표계를 직교 좌표계로 변환합니다.
				float x = r * Mathf.Sin(theta) * Mathf.Cos(fi);
				float y = r * Mathf.Sin(theta) * Mathf.Sin(fi);
				float z = r * Mathf.Cos(theta);

				transform.localPosition = new Vector3(x, y, z);
				transform.localRotation = Quaternion.LookRotation(
						-transform.localPosition);


        // 스폰 가능한지 여부 체크 및 스폰 처리
        if (Spawnable.Pool<Portal>.Instance.Count > 0)
            InstantiateSpawnable<Portal>();
        // ObjectToSpawn에 따라 적절한 Spawnable을 스폰
        SpawnObject();
	
        SpawnCount++;

        if (SpawnCount == SpawnLimit) StopSpawning();
    }
}
// 스폰 가능한 모든 프리팹을 배열에 저장합니다.

public GameObject[] EnermyPrefabs;
private GameObject previousSpawnedEnermyPrefab;

private void SpawnObject()
{
    // Check if there are prefabs in the array
    if (EnermyPrefabs.Length == 0)
    {
        Debug.LogError("EnermyPrefabs array is empty. Add prefabs to the array.");
        return;
    }

    // Choose a random index for the prefab
    int randomIndex = UnityEngine.Random.Range(0, EnermyPrefabs.Length);

    // Ensure a different prefab is chosen than the previously spawned one
    while (EnermyPrefabs.Length > 1 && EnermyPrefabs[randomIndex] == previousSpawnedEnermyPrefab)
    {
        randomIndex = UnityEngine.Random.Range(0, EnermyPrefabs.Length);
    }

    // Set the previously spawned prefab for the next iteration
    previousSpawnedEnermyPrefab = EnermyPrefabs[randomIndex];

    // Instantiate the chosen prefab
    GameObject spawnedObject = Instantiate(previousSpawnedEnermyPrefab, transform.position, Quaternion.identity);
	// objectToSpawn에 따라 적절한 Spawnable을 스폰하는 매소드
    if (objectToSpawn == typeof(Crasher))
        InstantiateSpawnable<Crasher>();
    else if (objectToSpawn == typeof(Drone))
        InstantiateSpawnable<Drone>();
    else if (objectToSpawn == typeof(BulletCapsule))
        InstantiateSpawnable<BulletCapsule>();
    else if (objectToSpawn == typeof(ArmorCapsule))
        InstantiateSpawnable<ArmorCapsule>();
    else if (objectToSpawn == typeof(HealthCapsule))
        InstantiateSpawnable<HealthCapsule>();
    else if (objectToSpawn == typeof(PowerUpCapsule))
        InstantiateSpawnable<PowerUpCapsule>();
    else
        throw new UnityException("Unrecognized type of Spawnable");}
	
    

private void InstantiateSpawnable<T>() where T : Spawnable
{
	// Spawnable 객체를 생성하고 위치 및 회전을 설정하여 활성화하는 메서드

    // Spawnable 객체를 객체 풀에서 가져옵니다.
    var spawned = Spawnable.Pool<T>.Instance.RequestObject();

	// 현재 Spawner의 위치 및 회전 정보를 가져와 Spawnable에 적용합니다.
    spawned.transform.position = transform.localPosition;
    spawned.transform.rotation = transform.localRotation;
	// Spawnable을 활성화합니다.
    spawned.gameObject.SetActive(true);
}

		public void StartSpawning() {
			// 스포너를 시작하는 매서드
			if (IsSpawning)
				throw new UnityException(
					"A spawner should not be restarted before stopping it first");
			//스폰 횟수 초기화 및 스포너 활성화 상태로 설정
			SpawnCount = 0;
			IsSpawning = true;
			//Coroutine을 사용하여 Spawn 매서드 비동기적으로 실행
			lastSpawnCall = StartCoroutine(Spawn());
		}

		public void StartSpawning(Type type, int limit, float rate,
					float delay, float minDistance, float maxDistance) {
			// 스폰에 필요한 설정값을 받아와서 스포너를 시작하는 메서드

            // 설정값으로 스포너를 구성
			Configure(type, limit, rate,
					delay, minDistance, maxDistance);
			// 스포너 시작
			StartSpawning();
		}

		public void StartSpawning(SpawnConfig config) {
			// SpawnConfig를 사용하여 스포너를 시작하는 메서드

			// 설정값으로 스포너를 구성
			Configure(config);
			// 스포너 시작
			StartSpawning();
		}

		public void StopSpawning() {
			//스포너를 중지하는 매서드

			// 이미 중지된 상태면 아무것도 하지 않음
			if (!IsSpawning) return;

			// 스포너 비활성화 상태로 설정 및 Coroutine 중지
			IsSpawning = false;
			StopCoroutine(lastSpawnCall);
		}

		public static Stack<SpawnConfig>[] ParseSpawnPattern(string spawnPatternFilePath, int level = 1) {
			// 스폰 패턴을 파싱하여 스폰 설정을 반환하는 매서드

			// 스폰 설정을 저장할 변수 초기화
			Type type = default;
			int limit = default, multiplier = -1;
			float rate = default, delay = default,
				  maxDistance = default, minDistance = default;
			
			// 현재 레벨에 대한 파싱이 완료되었는지 여부를 저장하는 변수
			bool doneParsingForCurrentLevel = false;

			// 각 타입에 대한 스폰 설정을 저장할 스택 및 딕셔너리 초기화
			var patterns = new Stack<SpawnConfig>();
			var groupsByType = new Dictionary<Type, Stack<SpawnConfig>>();

			while (!doneParsingForCurrentLevel) {
				//XML파일을 계속 읽어오거나 처음으로 돌아가는 부분
				if (!(xmlPattern?.Read() ?? false)) {
					xmlPattern = XmlReader.Create(spawnPatternFilePath);
					xmlPattern.MoveToContent();
				}

				// 레벨이 1 보다 큰 경우, 해당 레벨까지 이동

				if (level > 1) {
					xmlPattern.ReadToDescendant("level");
					for (int i = 1; i < level; i++) {
						xmlPattern.Skip();
					}
				}

				// 현재 노드의 타입에 따라 처리
				switch (xmlPattern.NodeType) {
					case XmlNodeType.Element:
						switch (xmlPattern.Name) {
							case "spawnable":
							    // "type" 속성을 기반으로 스폰할 타입 결정
								if (!xmlPattern.HasAttributes) {
									throw new UnityException(
										"Spawnable type not specified in pattern.");
								}

								switch (xmlPattern.GetAttribute("type")) {
									case nameof(Crasher):
										type = typeof(Crasher);
										break;
									case nameof(Drone):
										type = typeof(Drone);
										break;
									case nameof(BulletCapsule):
										type = typeof(BulletCapsule);
										break;
									case nameof(HealthCapsule):
										type = typeof(HealthCapsule);
										break;
									case nameof(ArmorCapsule):
										type = typeof(ArmorCapsule);
										break;
									case nameof(PowerUpCapsule):
										type = typeof(PowerUpCapsule);
										break;
									default:
										throw new UnityException(
											$"Error in {spawnPatternFilePath}:\n" +
											$"{xmlPattern.GetAttribute("type")} is not a" +
											" valid type of spawnable."
										);
								}
								break;

							case "pattern":
							    // "pattern" 엘리먼트인 경우
								if (xmlPattern.HasAttributes)
									multiplier = int.Parse(xmlPattern.GetAttribute("repeat"));
								if (multiplier <= 1)
									multiplier = 1;
								break;
							// 다양한 엘리맨트에 대한 처리
							case nameof(limit):
								limit = xmlPattern.ReadElementContentAsInt();
								break;
							case nameof(rate):
								rate = xmlPattern.ReadElementContentAsFloat();
								break;
							case nameof(delay):
								delay = xmlPattern.ReadElementContentAsFloat();
								break;
							case nameof(maxDistance):
								maxDistance = xmlPattern.ReadElementContentAsFloat();
								break;
							case nameof(minDistance):
								minDistance = xmlPattern.ReadElementContentAsFloat();
								break;
						}
						break;

					case XmlNodeType.EndElement
					// "pattern", "spawnable", "level" 등의 종료 엘리먼트에 대한 처리
					when xmlPattern.Name == "pattern":
					    // "pattern" 엘리먼트 종료시, 스폰 설정을 만들어 스택에 추가
						SpawnConfig pattern = new SpawnConfig(
							type, limit, rate, delay,
							minDistance, maxDistance
						);
						do patterns.Push(pattern); while (--multiplier > 0);
						break;

					case XmlNodeType.EndElement
					when xmlPattern.Name == "spawnable":
					    // "spawnable" 엘리먼트 종료시, 해당 타입에 대한 스택에 패턴 추가
						if (groupsByType.ContainsKey(type))
							foreach (var p in patterns)
								groupsByType[type].Push(p);
						else
							groupsByType.Add(type, new Stack<SpawnConfig>(patterns));
						patterns.Clear();
						break;

					case XmlNodeType.EndElement
					when xmlPattern.Name == "level":
					    // "level" 엘리먼트 종료시, 현재 레벨의 파싱 완료
						doneParsingForCurrentLevel = true;
						break;
				}
			}

			xmlPattern.Close();

			// 추출된 패턴을 배열로 변환하여 반환
			Stack<SpawnConfig>[] extractedPatterns = new Stack<SpawnConfig>[groupsByType.Count];
			groupsByType.Values.CopyTo(extractedPatterns, 0);
			return extractedPatterns;
		}

		public static void SpawnerFactory(
				Stack<SpawnConfig>[] spawnPatterns, int index,
				ref Dictionary<Type, List<Spawner>> spawners,
				ref Stack<Spawner> stashedSpawners) {

			List<Type> requiredSpawnerTypes = new List<Type>();

			bool recursed = false;

			for (int id = index; id < spawnPatterns.Length; id++) {
				Stack<SpawnConfig> pattern = spawnPatterns[id];
				Type type = pattern.Peek().type;

				// 필요한 스포너 타입을 추적
				if (!requiredSpawnerTypes.Contains(type))
					requiredSpawnerTypes.Add(type);

				// 현재 인덱스가 주어진 인덱스보다 작으면 건너뜁니다.
				if (id < index) continue;

				// 스포너의 재사용 및 생성로직
				if (!spawners.ContainsKey(type))
					spawners.Add(type, new List<Spawner>(0));

				int spawnersRequired = pattern.Count;
				int spawnersAvailable = spawners[type].Count;

				if (spawnersRequired > spawnersAvailable) {
					int spawnersReused;
					if (spawnersRequired <= spawnersAvailable + stashedSpawners.Count)
						spawnersReused = spawnersRequired - spawnersAvailable;
					else
						spawnersReused = stashedSpawners.Count;

					for (int i = 0; i < spawnersReused; i++) {
						spawners[type].Add(stashedSpawners.Pop());
						spawnersRequired--;
					}

					int recursionIndex = index + id + 1;
					if (spawnersRequired > 0 && recursionIndex < spawnPatterns.Length) {
						//재귀적으로 SpawnerFactory를 호출하여 추가적인 스포너 생성
						SpawnerFactory(spawnPatterns, recursionIndex,
									   ref spawners, ref stashedSpawners);

						recursed = true;

						// 재귀 호출로 인해 스포너가 재활용되었을 경우
						for (
							int i = stashedSpawners.Count;
							i != 0 && spawnersRequired > 0;
							i--
						) {
							spawners[type].Add(stashedSpawners.Pop());
							spawnersRequired--;
						}
					}

					while (spawnersRequired > 0) {
						// 필요한 수만큼 스포너를 생성하여 딕셔너리에 추가
						spawners[type].Add(Instantiate(
							Resources.Load<Spawner>(Prefabs.SPAWNER)));

						spawnersRequired--;
					}
				}

				// 스포너가 필요한 수보다 더 많이 생성된 경우
				else if (spawnersRequired < spawnersAvailable) {
					int firstLeftover = spawnersRequired + 1,
						leftoversCount = spawnersAvailable - spawnersRequired;
					
					// 남은 스포너를 스태쉬에 저장
					for (int i = firstLeftover; i < leftoversCount; i++)
						stashedSpawners.Push(spawners[type][i]);
					
					// 딕셔너리에서 남은 스포너 제거
					spawners[type].RemoveRange(firstLeftover, leftoversCount);
				}

				// 생성된 스포너에 패턴 설정 적용
				foreach (Spawner spawner in spawners[type]) {
					spawner.Configure(pattern.Pop());
				}

				// 특정 타입의 적에 대한 추가 설정
				// 타입이 Enemy의 하위 클래스인 경우
if (type.IsSubclassOf(typeof(Enemy))) {
    // 타입이 Crasher이고 Crasher의 인스턴스 수가 0인 경우
    if (type == typeof(Crasher) && Spawnable.Pool<Crasher>.Instance.Count == 0) {
        // Crasher 인스턴스를 생성합니다.
        Spawnable.Pool<Crasher>.Instance.Populate();
    }
    // 타입이 Drone이고 Drone의 인스턴스 수가 0인 경우
    else if (type == typeof(Drone) && Spawnable.Pool<Drone>.Instance.Count == 0) {
        // Drone 인스턴스를 생성합니다.
        Spawnable.Pool<Drone>.Instance.Populate();

        // EnemyBullet의 인스턴스 수가 0인 경우
        if (Spawnable.Pool<EnemyBullet>.Instance.Count == 0)
            // EnemyBullet 인스턴스를 생성합니다.
            Spawnable.Pool<EnemyBullet>.Instance.Populate();
    }

    // Portal의 인스턴스 수가 0인 경우
    if (Spawnable.Pool<Portal>.Instance.Count == 0)
        // Portal 인스턴스를 생성합니다.
        Spawnable.Pool<Portal>.Instance.Populate();
}
// 타입이 ArmorCapsule이고 ArmorCapsule의 인스턴스 수가 0인 경우
else if (type == typeof(ArmorCapsule) && Spawnable.Pool<ArmorCapsule>.Instance.Count == 0) {
    // ArmorCapsule 인스턴스를 생성합니다.
    Spawnable.Pool<ArmorCapsule>.Instance.Populate();
}
// 타입이 BulletCapsule이고 BulletCapsule의 인스턴스 수가 0인 경우
else if (type == typeof(BulletCapsule) &&Spawnable.Pool<BulletCapsule>.Instance.Count == 0) {
    // BulletCapsule 인스턴스를 생성합니다.
    Spawnable.Pool<BulletCapsule>.Instance.Populate();
}
// 타입이 HealthCapsule이고 HealthCapsule의 인스턴스 수가 0인 경우
else if (type == typeof(HealthCapsule) && Spawnable.Pool<HealthCapsule>.Instance.Count == 0) {
    // HealthCapsule 인스턴스를 생성합니다.
    Spawnable.Pool<HealthCapsule>.Instance.Populate();
}
// 타입이 PowerUpCapsule이고 PowerUpCapsule의 인스턴스 수가 0인 경우
else if (type == typeof(PowerUpCapsule) && Spawnable.Pool<PowerUpCapsule>.Instance.Count == 0) {
    // PowerUpCapsule 인스턴스를 생성합니다.
    Spawnable.Pool<PowerUpCapsule>.Instance.Populate();
}


				// 재귀적으로 호출된 경우 함수 종료
				if (recursed) return;
			}

			// 필요한 스포너 타입이 딕셔너리에 없는 경우 스태쉬에 저장된 스포너를 제거하고 딕셔너리에서 해당 타입 제거
			foreach (var type in requiredSpawnerTypes) {
				if (!spawners.ContainsKey(type)) {
					for (int i = 0; i < spawners[type].Count; i++)
						stashedSpawners.Push(spawners[type][i]);
					spawners.Remove(type);
				}
			}
		}
	}
}