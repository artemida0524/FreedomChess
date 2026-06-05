using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Game.Core.Sounds;
using Game.Runtime.Core.Auth;
using Game.Runtime.Gameplay;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;


public class BattleManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup battleCanvasGroup;
    [SerializeField] private FriendsPanel friendPanel;
    [SerializeField] private FriendButtonView friendButton;
    [SerializeField] private EnemyStatsView enemyStatsView;
    [SerializeField] private DropPanelBattle dropPanelBattle;
    [SerializeField] private ExitBattleButton exitBattleButton;
    [SerializeField] private ExitBattlePanel exitBattlePanel;


    public static BattleManager Instance { get; private set; }

    public ChessFigure SelectedFigure { get; set; }

    public bool InGame { get; set; } = false;
    public string BattleId { get; set; }

    private FirebaseDatabase _database;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Inject] private IPlayerAuthRepository _authRepository;

    public void Init()
    {
        ChessBoard.Instance.Init();
        ChessBoard.Instance.ResetBoard(0);
        _database = FirebaseDatabase.DefaultInstance;
    }

    public void EnemyFigureMove(string figureId, string cellId, string sound)
    {
        Debug.Log("Move Figure");
        ChessBoard.Instance.MoveEnemyFigure(figureId, cellId);
        if (sound == string.Empty)
        {
            return;
        }
        else if (sound == "move")
        {
            SoundManager.Instance.Move();
        }
        else if(sound == "take")
        {
            SoundManager.Instance.Take();
        }
    }

    public async void StartBattle(string enemyId, string battleId)
    {
        battleCanvasGroup.interactable = false;

        DataSnapshot snapshot = await _database.GetReference($"battle/{battleId}").GetValueAsync();

        BeginBattleData battleData = JsonUtility.FromJson<BeginBattleData>(snapshot.GetRawJsonValue());

        if (battleData.white == FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        {
            ChessBoard.Instance.ResetBoard(0);
        }
        else
        {
            ChessBoard.Instance.ResetBoard(1);
        }

        battleCanvasGroup.interactable = true;
        BattleRequestsListener.Instance.SetActive(false);
        Debug.Log($"{battleId}");
        _database.GetReference($"battle/{battleId}/moves").ChildAdded += BattleManager_ChildAdded;
        _database.GetReference($"battle/{battleId}/battleOver").ValueChanged += BattleManager_ValueChanged;
        exitBattlePanel.OnYesClicked += ExitBattlePanel_OnYesClicked;
        enemyStatsView.View(enemyId);
        dropPanelBattle.SetActive(false);
        friendButton.SetActive(false);
        exitBattleButton.SetActive(true);
        InGame = true;
        BattleId = battleId;
        SoundManager.Instance.StartBattle();
    }

    private async void ExitBattlePanel_OnYesClicked()
    {
        battleCanvasGroup.interactable = false;

        await _database.GetReference($"battle/{BattleId}/battleOver").SetValueAsync("yes");

        battleCanvasGroup.interactable = true;
    }

    private void BattleManager_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (!e.Snapshot.Exists)
        {
            return;
        }
        if ((string)e.Snapshot.Value == "yes")
        {

            _database.GetReference($"battle/{BattleId}/moves").ChildAdded -= BattleManager_ChildAdded;
            _database.GetReference($"battle/{BattleId}/battleOver").ValueChanged -= BattleManager_ValueChanged;

            enemyStatsView.Hide();
            ChessBoard.Instance.ResetBoard(0);
            dropPanelBattle.SetActive(true);
            friendButton.SetActive(true);
            exitBattleButton.SetActive(false);
            exitBattlePanel.SetActive(false);
            InGame = false;
            BattleId = string.Empty;
        }
        else if ((string)e.Snapshot.Value == "no")
        {
            return;
        }
    }

    private void BattleManager_ChildAdded(object sender, ChildChangedEventArgs e)
    {
        Debug.Log("first");
        if (!e.Snapshot.Exists)
        {
            Debug.Log("second");
            return;
        }
        ChessMoveData chessMoveData = JsonUtility.FromJson<ChessMoveData>(e.Snapshot.GetRawJsonValue());
        if (chessMoveData.uid == FirebaseAuth.DefaultInstance.CurrentUser.UserId)
        {
            Debug.Log("third");
            return;
        }

        EnemyFigureMove(chessMoveData.chessFigureId, chessMoveData.chessCellId, chessMoveData.sound);
    }

    public async void MeMove(ChessFigure selected, ChessCell chessCell, string sound)
    {
        if (!InGame)
        {
            return;
        }

        Guid moveId = Guid.NewGuid();
        ChessMoveData moveData = new ChessMoveData
        {
            uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId,
            chessFigureId = selected.figureName,
            chessCellId = chessCell.position,
            sound = sound
        };
        string json = JsonUtility.ToJson(moveData);

        await _database.GetReference($"battle/{BattleId}/moves/{moveId.ToString()}").SetRawJsonValueAsync(json);

    }

    [Serializable]
    public class ChessMoveData
    {
        public string uid;
        public string chessFigureId;
        public string chessCellId;
        public string sound;
    }

    public async void WaitPlayer(string uid, string battleId)
    {

        battleCanvasGroup.interactable = false;

        friendPanel.WaitPlayer();

        bool result = await WaitForPlayerAndDiscard(battleId, 6);

        if (!result)
        {
            battleCanvasGroup.interactable = true;
            return;
        }

        StartBattle(uid, battleId);

        battleCanvasGroup.interactable = true;
    }

    private async UniTask<bool> WaitForPlayerAndDiscard(string battleId, float timeoutSeconds)
    {
        float elapsed = 0f;
        float interval = 0.5f;

        while (elapsed < timeoutSeconds)
        {
            DataSnapshot snap = await _database
                .GetReference($"battle/{battleId}/enemyPlayerResponse")
                .GetValueAsync();

            string value = snap.Value as string;

            if (value == "yes")
            {
                return true;
            }

            if (value == "no")
            {
                return false;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(interval));
            elapsed += interval;
        }

        return false;
    }


}
