# 🎨 Fusion Bomber Asset Guide

이 가이드는 게임 프로토타입에 필요한 시각적 에셋의 규격과 배치 경로를 안내합니다.

## 📁 에셋 배치 경로 (Assets/_Project/Textures/...)

1. **Player 캐릭터** (`Player_Sheet.png`)
   - 2D Sprite (Multiple) 추천
   - 규격: 128x128 픽셀 (또는 64x64)
   - 애니메이션: Idle, Walk, Win, Lose

2. **폭탄** (`Bomb.png`)
   - 규격: 64x64 픽셀
   - 틱-톡 거리는 카운트다운 애니메이션이 있으면 좋습니다.

3. **폭발 효과** (`Explosion.png`)
   - 십자 형태의 불길 (Center, Side, Tail 조각 필요)
   - 규격: 각 조각당 64x64 픽셀

4. **맵 타일** (`Map_Tileset.png`)
   - 64x64 단위의 타일셋 (Tilemap 사용 추천)
   - 바닥(Floor), 파괴 가능 블록(Soft Wall), 파괴 불가 벽(Hard Wall)

## 🛠 유니티 에디터 설정 순서 (코드 작성 후 필수 작업)

1. **Photon Fusion 2 임포트**: 에셋 스토어에서 다운로드하여 프로젝트에 넣으세요.
2. **NetworkRunner 프리팹 생성**: `NetworkGameManager`에 할당할 빈 오브젝트(`NetworkRunner` 컴포넌트 포함)를 프리팹으로 만드세요.
3. **Player 프리팹 구성**:
   - `NetworkObject`, `NetworkCharacterControllerPrototype` 컴포넌트 추가
   - `PlayerController.cs` 스크립트 연결
4. **Bomb 프리팹 구성**:
   - `NetworkObject` 컴포넌트 추가
   - `BombController.cs` 스크립트 연결
   - `wallLayer`를 'Wall' 레이어로 설정하세요.

## 🎮 실행 팁
- `ParrelSync`를 사용하기 전, 반드시 **Project Settings > Player > Resolution and Presentation > Run In Background**를 체크해야 두 클라이언트가 동시에 돌아갑니다.
