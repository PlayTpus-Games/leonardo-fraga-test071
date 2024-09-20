using UnityEngine;

[RequireComponent(typeof(PreGameCardFlipper))]
public class GameplayInitializer : MonoBehaviour
{
    public static GameplayInitializer Instance;

    private PreGameCardFlipper _preGame;
    private IUnloadable[] _iReloadableScripts;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        _preGame = GetComponent<PreGameCardFlipper>();
        _iReloadableScripts = GetComponents<IUnloadable>();
    }

    public void Unload()
    {
        foreach (IUnloadable reloadable in _iReloadableScripts)
            reloadable.Unload();
    }
    
    public void StartLevel(CardHolder cardHolder) => _preGame.InitializeGameplay(cardHolder);
}
