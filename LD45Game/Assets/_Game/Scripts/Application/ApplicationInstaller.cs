using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ApplicationInstaller : MonoInstaller {
    public override void InstallBindings() {
        InstallSignals(Container);
    }

    private void InstallSignals(DiContainer container) {
        SignalBusInstaller.Install(container);
    }
}
