# Fusion Bomber (1vs1 Grid-based Multiplayer)

Unity 6.4와 Photon Fusion을 활용한 1vs1 실시간 멀티플레이어 폭탄 대전 게임 프로토타입입니다.

## 🛠 기술 스택
- **Engine:** Unity 6.4.0f1 (Recommended)
- **Network:** Photon Fusion 2 (Host Mode)
- **Architecture:** Server-Authoritative (Host-based)
- **Testing:** ParrelSync

## 🚀 시작 가이드
1. **Photon Dashboard:** [Photon Engine](https://dashboard.photonengine.com/)에서 'Fusion' 앱 ID를 생성합니다.
2. **Setup:** Unity 프로젝트 내 `Photon/Fusion/Resources/PhotonAppSettings`에 App ID를 입력합니다.
3. **Scenes:** `Main` 씬에 `NetworkGameManager` 오브젝트를 배치하고 스크립트를 연결합니다.

## 🗺 로드맵
- [x] Host/Client 매치메이킹 시스템
- [x] 격자 기반 서버 권한 이동 시스템
- [x] 서버 판정 폭탄 설치 및 폭발 로직
- [ ] 아이템 및 블록 파괴 시스템 (TBA)
- [ ] 승리/패전 UI 연출 (TBA)