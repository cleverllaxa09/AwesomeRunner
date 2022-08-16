﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour,IResettable
{
    public static GameSession Instance { get; private set; } 

    [SerializeField] private Player currentPlayer;
    private IInputTranslator inputTranslator;

    private bool isSessionPaused = false;
    private bool isInputAlreadyRestricted = false;
    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Update()
    {
       inputTranslator.Tick();
    }

    private void Init()
    {
        if (ApplicationUtil.platform == RuntimePlatform.Android || ApplicationUtil.platform == RuntimePlatform.IPhonePlayer)
        {
            IBindingHolder<TouchBinding> touchHolder = new TouchBindingHolder();
            inputTranslator = new InputTranslator<TouchBinding>(touchHolder);
        }
        else
        {
            IBindingHolder<KeyBinding> keyHolder = new KeyBindingHolder();
            inputTranslator = new InputTranslator<KeyBinding>(keyHolder);
        }
    }

    public void AddCommandTranslator(ICommandTranslator translator)
    {
        inputTranslator.AddCommandTranslator(translator);
    }

  
    public void PauseSession(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
        if (!isSessionPaused && inputTranslator.IsTranslationResticted(InputConstants.InGameCommands))
        {
            isInputAlreadyRestricted = true;
            isSessionPaused = isPaused;
            return;
        }
        if (!inputTranslator.IsTranslationResticted(InputConstants.InGameCommands))
        {
            isInputAlreadyRestricted = false;
        }
        isSessionPaused = isPaused;
        if (isInputAlreadyRestricted)
        {
            return;
        }   
        RestrictInputs(InputConstants.InGameCommands,isRestricted: isPaused);
    }

    public void RestrictInputs(List<ECommand> commands,bool isRestricted)
    {
        inputTranslator.RestictTranslation(commands, isRestricted);
    }

    public void RestartSession()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        ResetToDefault();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        PauseSession(false);
        currentPlayer.ResetToDefault();
    }
}