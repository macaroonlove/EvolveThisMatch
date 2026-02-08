# 🛡️ EvolveThisMatch (이번판만 키우기)
> **방치형 로그라이크 디펜스**

---

## 1️⃣ Manager & System
전체 게임의 흐름을 제어하는 계층입니다.  
**Core**는 모든 씬에서 재사용되는 범용 시스템을, **Battle**은 전투 씬의 독립적인 로직과 규칙을 담당합니다.

### 🔹 Core Manager
어떠한 씬에서도 파괴되지 않고 공통으로 사용되는 핵심 매니저 그룹입니다.

| 시스템 명칭 | 설명 |
| :--- | :--- |
| **[PoolSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Core/PoolSystem.cs)** | Dictionary와 Stack을 결합하여 가변적인 오브젝트 풀링을 관리하는 시스템 |
| **[GlobalStatusSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Core/GlobalStatusSystem.cs)** | 게임 전역의 상태(플레이 시간, 현재 스테이지, 유저 진행도 등)를 관리하는 시스템 |
| **[ArtifactSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Core/ArtifactSystem.cs)** | 보유 중인 패시브 아이템(아티팩트)들의 효과를 데이터화하여 적용시키는 시스템 |
| **[TomeSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Core/TomeSystem.cs)** | 사용자가 직접 사용하는 액티브 아이템(고서)의 효과 및 쿨타임을 관리하는 시스템 |

### 🔹 Battle Manager
전투 씬 진입 시 활성화되며, 유닛의 생명주기 및 전투 규칙을 제어하는 시스템 그룹입니다.

| 시스템 명칭 | 설명 |
| :--- | :--- |
| **[AllySystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/AllySystem.cs)** | 현재 필드에 배치된 모든 아군 유닛의 리스트와 상태를 관리하는 클래스 |
| **[AgentCreateSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/AgentCreateSystem.cs)** | 유닛 소환 로직을 담당하며, 초기 배치 및 보충 시 유닛을 생성 |
| **[AgentReturnSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/AgentReturnSystem.cs)** | 사망 혹은 교체 시 유닛을 오브젝트 풀로 안전하게 반환 |
| **[AgentChangeSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/AgentChangeSystem.cs)** | 전투 중 유닛의 교체 및 엔트리 변경 로직 담당 |
| **[AgentLevelSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/AgentLevelSystem.cs)** | 전투 내 유닛 레벨에 따른 스탯 상승 및 효과 반영 |
| **[AgentTalentSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/AgentTalentSystem.cs)** | 유닛별 고유 '재능' 효과를 연산하고 전투 로직에 주입 |
| **[AgentSyncSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Battle/System/AgentSyncSystem.cs)** | 전투 중 일시적인 강화 및 유닛 간 싱크로율 효과 관리 |
| **[AgentLimitSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Battle/System/AgentLimitSystem.cs)** | 유닛 승격(Limit Break)에 따른 능력치 변화 및 시각 효과 관리 |
| **[SummonCreateSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/SummonCreateSystem.cs)** | 유닛에 귀속된 소환수들의 생성 및 소유권 관리 |
| **[EnemySystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/EnemySystem.cs)** | 현재 필드에 존재하는 모든 적 유닛을 전역적으로 관리 |
| **[EnemySpawnSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/EnemySpawnSystem.cs)** | 적 유닛의 스폰 위치 및 타이밍을 제어 |
| **[WaveSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/WaveSystem.cs)** | 스테이지별 적 유닛의 구성과 웨이브 진행 흐름 관리 <br> (전투용 `BattleWaveSystem`, 로비용 `LobbyWaveSystem`) |
| **[TileSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/TileSystem.cs)** | 아군 유닛이 배치될 타일의 위치 정보 및 점유 상태 관리 |
| **[BlockSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/BlockSystem.cs)** | 적 유닛의 진행을 물리적으로 막는 바리케이드 상태 및 파괴 로직 관리 |
| **[UnitRayCastSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Battle/System/UnitRayCastSystem.cs)** | 레이캐스트를 통해 유닛 선택 여부를 파악하고 결과를 UnityAction 콜백으로 전달 |
| **[BattleResultSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Battle/System/BattleResultSystem.cs)** | 승리/패배 조건을 실시간 체크하여 전투 결과를 판정 |
| **[CoinSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Battle/System/CoinSystem.cs) / [CrystalSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Battle/System/CrystalSystem.cs)** | 전투 내에서만 유효한 임시 재화의 획득 및 소비 로직 관리 |
| **[ElementalSystem](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/System/Battle/ElementalSystem.cs)** | 전투 중 변화하는 속성 레벨과 그에 따른 상성 보너스 효과 관리 |

---

## 2️⃣ Unit & Ability
유닛은 독립적인 **Ability**들의 집합체로 구성됩니다. 모든 행동은 원자 단위로 쪼개져 **[Unit.cs](링크.cs)**에서 전역적으로 관리됩니다.

### 🔹 Always Ability
| 어빌리티 명칭 | 설명 |
| :--- | :--- |
| **[MoveAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/MoveAbility.cs)** | 유닛의 이동 제어 (추적용 `MoveChase`, 경계 이동용 `MoveBoundary`) |
| **[AttackAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/AttackAbility.cs)** | 공격 매커니즘 실행 (공속/사거리 연산 및 트리거 발성) |
| **[ActiveSkillAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/ActiveSkillAbility.cs)** | 액티브 스킬의 인스턴스 관리 및 런타임 실행 제어 |
| **[DeployAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/DeployAbility.cs)** | 아군 유닛의 배치 및 전장 출전 상태 실시간 동기화 |

### 🔹 Condition Ability
| 어빌리티 명칭 | 설명 |
| :--- | :--- |
| **[HitAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/HitAbility.cs)** | 회피력 계산 및 피해 면역 체크 후 데이터 전달 |
| **[DamageCalculateAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/DamageCalculateAbility.cs)** | 치명타, 관통력, 방어력 등을 종합한 최종 데미지 산출 |
| **[HealthAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/HealthAbility.cs)** | 최대 체력 연산 및 HP 증감(회복/피해) 조작 |
| **[PassiveSkillAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/PassiveSkillAbility.cs)** | 유닛이 보유한 상시 적용 패시브 데이터 및 효과 유지 |
| **[BuffAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/BuffAbility.cs)** | 유닛에게 적용된 버프의 지속 시간 및 중첩 제어 |
| **[AbnormalStatusAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/AbnormalStatusAbility.cs)** | 상태이상(CC기) 및 부정적 효과의 상태 머신 관리 |
| **[EntitySpawnAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/EntitySpawnAbility.cs)** | 투사체, 덫, 소환수 등 외부 엔티티 생성 트리거 |
| **[FindTargetAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/FindTargetAbility.cs)** | 시스템을 참조하여 최적의 타겟을 찾는 유틸리티 제공 |
| **[FXAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/FXAbility.cs)** | 유닛에 적용된 파티클 및 전용 셰이더 상태 관리 |
| **[UnitAnimationAbility](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Unit/Ability/UnitAnimationAbility.cs)** | 유닛 상태에 따른 애니메이션 동기화 및 제어 |

---

## 3️⃣ Effect Pipeline & MutableValue
모든 게임 내 상호작용(데미지, 버프, 상태변화)은 **Effect** 단위를 통해 데이터 중심으로 처리됩니다.  
각 요소는 실행 계층에 따라 유기적으로 연결됩니다.


### 🔹 Effect 구조 및 프로세스
효과는 발동 방식과 실행 로직에 따라 세 가지 계층으로 구분됩니다.

1. **[발동 계층 (Trigger & DeliveryEffect)](https://github.com/macaroonlove/EvolveThisMatch/tree/main/Assets/EvolveThisMatch/Script/Core/Effects/Delivery)**
   - **Trigger**: 특정 조건(OnAttack, OnHit 등)이 만족될 때 자동으로 효과를 발생시키며, 내부적으로 `DeliveryEffect`를 호출합니다.
   - **DeliveryEffect**: 효과가 대상에게 전달되는 '방식'(투사체, 즉시 전달, 범위 전파 등)을 정의합니다. 수동 발동이 필요할 때 직접 호출되는 단위이기도 합니다.

2. **[실행 계층 (ExecuteEffect)](https://github.com/macaroonlove/EvolveThisMatch/tree/main/Assets/EvolveThisMatch/Script/Core/Effects/Execute)**
   - **ExecuteEffect**: 실제 수치 연산 및 물리적 로직이 실행되는 핵심 단계입니다.
   - **Logic Unit**: 자주 사용되는 효과들을 원자 단위의 `Logic`으로 모듈화하여 재사용성을 극대화하고 중복 코드를 방지했습니다.

3. **[데이터 계층 (DataEffect)](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Effects/Status/Data/DataEffect.cs)**
   - **DataEffect**: 버프나 상태이상처럼 특정 기간 동안 유지되어야 하는 '상태 데이터'입니다. 효과의 지속 시간, 중첩 여부, 종료 시의 회수 로직을 전담합니다.

### 🔹 [MutableValue (가변 수치 시스템)](https://github.com/macaroonlove/EvolveThisMatch/blob/main/Assets/EvolveThisMatch/Script/Core/Effects/MutableValue.cs)
방치형 게임의 기하급수적인 성장을 제어하기 위한 핵심 클래스입니다.
- **구조**: `BaseValue`와 복수의 `Modifier`(합연산, 곱연산 등)를 결합하여 최종값을 산출합니다.
- **동적 연산**: 유닛의 레벨, 속성, 외부 버프에 따라 실시간으로 값이 변하며, 값 변경 시 이벤트를 통해 UI나 시스템에 통지하여 연산 부하를 최소화합니다.

---

## 4️⃣ UI & UX (Architecture)
팀 프로젝트에서의 안정성과 유지보수 효율을 위해 엄격한 UI 아키텍처를 도입했습니다.

- **UI Binding**: 런타임 중 필요한 요소를 단 한 번만 로드하고 캐싱합니다. 이를 통해 **갱신 누락**을 방지하고, 씬 파일 직접 수정에 의한 **Git 충돌(Conflict)** 가능성을 최소화했습니다.
- **MVP (Model-View-Presenter) Pattern**:
    - **Model**: 순수 데이터 및 서버 동기화 값 관리.
    - **View**: 수동적인 UI 출력(애니메이션, 텍스트) 전담.
    - **Presenter**: 둘 사이의 다리 역할을 수행하여 **결합도는 낮추고 응집도는 높임**.

---

## 5️⃣ Lobby
전투 외적인 유닛의 성장과 전반적인 영지 관리를 담당하는 게임 섹션입니다. 각 시스템은 **PlayFab** 서버 데이터와 연동되어 데이터 무결성을 유지합니다.

### 🔹 유닛 성장 및 강화
| 시스템 명칭 | 설명 | 핵심 로직 및 특징 |
| :--- | :--- | :--- |
| **LevelUp** | 유닛 기본 레벨업 | 중복 유닛 소모를 통한 레벨 상승 |
| **TierUp** | 유닛 승격 | 최대 레벨 확장 및 다양한 추가 효과 제공 |
| **Talent** | **유닛 고유 재능** | 개별 유닛의 랜덤한 능력을 해금하는 성장 시스템 |

### 🔹 아이템
| 시스템 명칭 | 설명 | 핵심 로직 및 특징 |
| :--- | :--- | :--- |
| **Artifact** | 패시브 아이템 관리 | 보유 시 전역 유닛에게 상시 적용되는 `Modifier` 데이터 관리 |
| **Tome** | 액티브 고서 관리 | 전투 중 플레이어가 직접 개입하는 스킬 아이템의 사용 조건 및 효과 처리 |

### 🔹 영지 및 경제 시스템
| 시스템 명칭 | 설명 | 핵심 로직 및 특징 |
| :--- | :--- | :--- |
| **Idle** | 방치형 보상 제공 | 온라인·오프라인 시간당 방치형 보상 제공 |
| **Department** | 영지 및 건물 관리 | 자원 생산 건물의 레벨에 따른 시간당 방치형 보상 제공 |
| **Shop** | 상점 | 인게임 재화 및 인앱 결제(IAP)를 통한 아이템 구매 |
| **Gacha** | 소환 | 가차 확률 테이블 기반의 유닛/아이템 획득 및 결과 데이터 서버 동기화 |

---
