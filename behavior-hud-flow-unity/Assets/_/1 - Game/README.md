# Flow

## Bootstrap

- AppCenter SDK initialization
- Easy Mobile initialization

- Addressble

## ECS

Basic concept

Group - Initialization
Group - Simulation
Group - Presentation



app hud
app assist hud

preparation hud
preparation assist hud

stage hud
stage assist hud

app hud + [app assist hud] + preparation hud + [preparation assist hud]
app hud + [app assist hud] + stage hud + [stage assist hud]

should be combined to for command stream

- InitializeAppwideServiceSystem
  - LoadAppHudSystem
  - LoadAppAssistHudSystem
- SetupAppwideServiceSystem
- CleanupAppwideServiceSystem(Most of the time, this won't be executed)

```cs
public interface ICommandStreamProducer
{
    IObservable<int> CommandStream { get; }
}

public class AppHud : ICommandStreamProducer
{
}

public class AppAssistHud : ICommandStreamProducer
{
}

public interface ITalkStepPlayer
{
    IObservable<TalkStepMessage> PlayingStream { get; }
}

public class Bootstrap : ICommandService
{
    //
    public readonly List<ICommandStreamProducer> _commandStreamProducers = new List<ICommandStreamProducer>();

    public IObservable<int> CommandStream => _rpTalkSteps.Select(x => x).Switch();
    private readonly ReactiveProperty<IObservable<int>> _rpCommands =
        new ReactiveProperty<IObservable<int>>(Observable.Empty<int>());


    private readonly List<ITalkStepPlayer> _talkStepPlayers = new List<ITalkStepPlayer>();

    public IObservable<TalkStepMessage> TalkStepStream => _rpTalkSteps.Select(x => x).Switch();

    private readonly ReactiveProperty<IObservable<TalkStepMessage>> _rpTalkSteps =
        new ReactiveProperty<IObservable<TalkStepMessage>>(Observable.Empty<TalkStepMessage>());

    void Start()
    {
        var combinedObs =
            _rpCommands
                .Select(x => x.CommandStream)
                .Aggregate(Observable.Empty<int>(), (acc, next) => acc.Merge(next));
        var combinedObs =
            _talkStepPlayers
                .Select(x => x.PlayingStream)
                .Aggregate(Observable.Empty<TalkStepMessage>(), (acc, next) => acc.Merge(next));
    }
}
```

- InitializePreparationwideServiceSystem
  - LoadPreparationHudSystem
- SetupPreparationwideServiceSystem
- CleanupPreparationwideServiceSystem

- InitializeStagewideServiceSystem
  - LoadStageHudSystem
  - LoadStageEnvironmentSystem
    - Load Zone Template
    - Load Zone Scenes
  - LoadStageUnitSystem
  - LoadStageCutSceneSystem
- SetupStagewideServiceSystem
- CleanupStagewideServiceSystem





Preparation <-> Stage

Preparation

- Hud

Stage

- Hud
- Environment

Some systems need to wait others completed so the setup can be performed.












- SceneManageSystem

SceneManageSystem

Store current scene

## State


- Preparation
  - Entering
  - Exiting
- Stage
  - Entering
  - Exiting

- SetupPreparationSceneSystem
- CleanupPreparationSceneSystem

### SetupPreparationSceneSystem

event EnteringPreparationScene

Load Scene - Preparation additive

### CleanupPreparationSceneSystem

event ExitingPreparationScene

Unload Scene - Preparation

- SetupPreparationHudSystem

event LoadingPreparationHud

- SetupStageSceneSystem
- CleanupStageSceneSystem

### SetupStageSceneSystem

event EnteringStageScene

### CleanupStageSceneSystem

event ExitingStageScene

- SetupStageHudSystem
- SetupStageEnvironmentSystem
- SetupStageCustSceneSystem