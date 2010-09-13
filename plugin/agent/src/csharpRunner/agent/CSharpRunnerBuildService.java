/*
 * Copyright 2000-2010 JetBrains s.r.o.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package csharpRunner.agent;

import csharpRunner.common.Util;
import jetbrains.buildServer.RunBuildException;
import jetbrains.buildServer.agent.AgentRunningBuild;
import jetbrains.buildServer.agent.artifacts.ArtifactsWatcher;
import jetbrains.buildServer.agent.runner.CommandLineBuildService;
import jetbrains.buildServer.agent.runner.ProcessListener;
import jetbrains.buildServer.agent.runner.ProgramCommandLine;
import jetbrains.buildServer.agent.runner.SimpleProgramCommandLine;
import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by IntelliJ IDEA.
 * User: Simone
 * Date: 11-set-2010
 * Time: 16.14.33
 * To change this template use File | Settings | File Templates.
 */
public class CSharpRunnerBuildService extends CommandLineBuildService {
    private final ArtifactsWatcher artifactsWatcher;

    public CSharpRunnerBuildService(ArtifactsWatcher artifactsWatcher) {
        this.artifactsWatcher = artifactsWatcher;
    }

    @NotNull
    @Override
    public List<ProcessListener> getListeners() {
        ArrayList<ProcessListener> listenerArrayList = new ArrayList<ProcessListener>(super.getListeners());
        listenerArrayList.add(new WriteToFileProcessListener(getBuild(), artifactsWatcher));
        
        return listenerArrayList;
    }

    @NotNull
    @Override
    public ProgramCommandLine makeProgramCommandLine() throws RunBuildException {
        AgentRunningBuild build = getBuild();
        
        return new SimpleProgramCommandLine(build,
                build.getAgentConfiguration().getAgentPluginsDirectory() +  "\\" + Util.NAME + "\\bin\\ciao.exe",
                new ArrayList<String>());
    }
}
