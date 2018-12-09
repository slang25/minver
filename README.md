<img src="assets/minver.png" width="100" />

# MinVer

_[![nuget](https://img.shields.io/nuget/v/MinVer.svg?style=flat&label=nuget%3A%20MinVer)](https://www.nuget.org/packages/MinVer)_
_[![nuget](https://img.shields.io/nuget/v/minver-cli.svg?style=flat&label=nuget%3A%20minver-cli)](https://www.nuget.org/packages/minver-cli)_
_[![Build status](https://ci.appveyor.com/api/projects/status/0ai8j3x4tg6w3ima/branch/master?svg=true)](https://ci.appveyor.com/project/adamralph/minver/branch/master)_
_[![Build Status](https://travis-ci.org/adamralph/minver.svg?branch=master)](https://travis-ci.org/adamralph/minver)_

A minimalistic [.NET package](https://www.nuget.org/packages/MinVer) for versioning .NET SDK-style projects using Git tags.

Platform support: all platforms supported by .NET SDK-style projects.

Also available as a [command line tool](#can-i-use-minver-to-version-software-which-is-not-built-using-a-net-sdk-style-project) for use in any Git repository.

- [Prerequisites](#prerequisites)
- [Quick start](#quick-start)
- [Usage](#usage)
- [How it works](#how-it-works)
- [Version numbers](#version-numbers)
- [Options](#options)
- [FAQ](#faq)

## Prerequisites

- [.NET Core SDK 2.1.300 or later](https://www.microsoft.com/net/download)
- [libcurl](https://curl.haxx.se/libcurl/) (Linux only)

## Quick start

- Install [MinVer](https://www.nuget.org/packages/MinVer).
- Build your project.

Your project will be versioned according to the latest tag found in the commit history.

## Usage

When you want to release a version of your software, whether it's a pre-release, RTM, patch, or anything else, simply create a tag with a name which is a valid [SemVer 2.0](https://semver.org/spec/v2.0.0.html) version and build your projects. MinVer will apply the version to the assemblies and packages. (If you like to prefix your tag names, see the [FAQ](#can-i-prefix-my-tag-names).)

## How it works

When the current commit is tagged with a version, the tag is used as-is.

When the current commit is not tagged, MinVer searches the commit history for the latest tag. If the latest tag found is a [pre-release](https://semver.org/spec/v2.0.0.html#spec-item-9), MinVer will use it as-is. If the latest tag found is RTM (not pre-release), MinVer will increase the patch number and add default pre-release identifiers, e.g. `1.0.0` becomes `1.0.1-alpha.0`. If no tag is found, the default version `0.0.0-alpha.0` is used.

You will notice that MinVer adds another number to the pre-release identifiers when the current commit is not tagged. This is the number of commits since the latest tag, or if no tag was found, since the root commit. This is known as "height". For example, if the latest tag found is `1.0.0-beta.1`, at a height of 42 commits, the calculated version is `1.0.0-beta.1.42`.

## Version numbers

MinVer sets the following custom properties:

- `MinVerVersion`
- `MinVerMajor`
- `MinVerMinor`
- `MinVerPatch`
- `MinVerPreRelease`
- `MinVerBuildMetadata`

Those properties are used to set the following .NET SDK properties, satisfying the official [open-source library guidance for version numbers](https://docs.microsoft.com/en-ca/dotnet/standard/library-guidance/versioning#version-numbers):

Property | Value
-- | --
`AssemblyVersion` | `{MinVerMajor}.0.0.0`
`FileVersion` | `{MinVerMajor}.{MinVerMinor}.{MinVerPatch}.0`
`PackageVersion` | `{MinVerVersion}`
`Version` | `{MinVerVersion}`

This behaviour can be [customised](#can-i-use-the-version-calculated-by-minver-for-other-purposes).

## Options

Options can be specified as either MSBuild properties or environment variables.

- [`MinVerBuildMetadata`](#can-i-include-build-metadata-in-the-version)
- [`MinVerMinimumMajorMinor`](#can-i-bump-the-major-or-minor-version)
- [`MinVerTagPrefix`](#can-i-prefix-my-tag-names)
- [`MinVerVerbosity`](#can-i-control-the-logging-verbosity)

Note that the option names are case-insensitive.

## FAQ

_(With TL;DR answers inline.)_

- [Can I bump the major or minor version?](#can-i-bump-the-major-or-minor-version) _(yes)_
- [Can I use my own pre-release versioning scheme?](#can-i-use-my-own-pre-release-versioning-scheme) _(yes)_
- [Can I prefix my tag names?](#can-i-prefix-my-tag-names) _(yes)_
- [Can I use my own branching strategy?](#can-i-use-my-own-branching-strategy) _(yes)_
- [Can I include build metadata in the version?](#can-i-include-build-metadata-in-the-version) _(yes)_
- [Can I use the version calculated by MinVer for other purposes?](#can-i-use-the-version-calculated-by-minver-for-other-purposes) _(yes)_
- [Can I control the logging verbosity?](#can-i-control-the-logging-verbosity) _(yes)_
- [Can I use MinVer to version software which is not built using a .NET SDK style project?](#can-i-use-minver-to-version-software-which-is-not-built-using-a-net-sdk-style-project) _(yes)_
- [What if the history diverges, and more than one tag is found?](#what-if-the-history-diverges-and-more-than-one-tag-is-found) _(nothing bad)_
- [What if the history diverges, and then converges again, before the latest tag (or root commit) is found?](#what-if-the-history-diverges-and-then-converges-again-before-the-latest-tag-or-root-commit-is-found) _(nothing bad)_
- [Why does MinVer fail with `LibGit2Sharp.NotFoundException`?](#why-does-minver-fail-with-libgit2sharpnotfoundexception) _(easy to fix)_
- [Why does MinVer fail with `System.TypeInitializationException`?](#why-does-minver-fail-with-systemtypeinitializationexception) _(easy to fix)_

### Can I bump the major or minor version?

Yes! You probably want to do this because at a point in time, on a given branch, you are working on a `MAJOR.MINOR` range, e.g. `1.0`, `1.1`, or `2.0`. The branch could be `master`, `develop`, a special release branch, a support branch, or anything else.

Before you create the first tag on that branch, interim builds will use the latest tag found in the commit history, which may not match the `MAJOR.MINOR` range which the current branch represents. Or if no tag is found in the commit history, interim builds will have the default version `0.0.0-alpha.0`. If you prefer those interim builds to have a version within the current range, specify the range with [`MinVerMinimumMajorMinor`](#options). For example:

```xml
<PropertyGroup>
  <MinVerMinimumMajorMinor>1.0</MinVerMinimumMajorMinor>
</PropertyGroup>
```

MinVer will now use a default version of `1.0.0-alpha.0`.

If you begin to release versions in the `1.0` range from another branch (e.g. a special release branch), update this value to `1.1`, `2.0`, or whatever `MAJOR.MINOR` range the current branch now represents.

Note that `MinVerMinimumMajorMinor` will be redundant after you create the first tag with same `MAJOR.MINOR`. If you don't care that the versions of interim builds before that first tag will have a lower `MAJOR.MINOR`, then simply don't specify `MinVerMinimumMajorMinor`.

Also note that if the latest tag found in the commit history has a higher `MAJOR.MINOR` than `MinVerMinimumMajorMinor`, then `MinVerMinimumMajorMinor` will be ignored.

### Can I use my own pre-release versioning scheme?

Yes! MinVer doesn't care what your pre-release versioning scheme is. The default pre-release identifiers are `alpha.0`, but you can use whatever you like in your tags. If your versioning scheme is valid [SemVer 2.0](https://semver.org/spec/v2.0.0.html), it will work with MinVer.

For example, all these versions work with MinVer:

- `1.0.0-beta.1`
- `1.0.0-pre.1`
- `1.0.0-preview-20181104`
- `1.0.0-rc.1`

### Can I prefix my tag names?

Yes! Specify the prefix with [`MinVerTagPrefix`](#options).

For example, if you prefix your tag names with "v", e.g. `v1.2.3`:

```xml
<PropertyGroup>
  <MinVerTagPrefix>v</MinVerTagPrefix>
</PropertyGroup>
```

### Can I use my own branching strategy?

Yes! MinVer doesn't care about branches. It's all about the tags!

That means MinVer is compatible with [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/), [GitHub Flow](https://guides.github.com/introduction/flow/), [Release Flow](https://docs.microsoft.com/en-us/azure/devops/learn/devops-at-microsoft/release-flow), and any other exotic flow.

### Can I include build metadata in the version?

Yes! Specify [build metadata](https://semver.org/spec/v2.0.0.html#spec-item-10) with `MinVerBuildMetadata`.

For example, in [`appveyor.yml`](https://www.appveyor.com/docs/appveyor-yml/):

```yaml
environment:
  MINVERBUILDMETADATA: build.%APPVEYOR_BUILD_NUMBER%
```

You can also specify build metadata in a version tag. If the tag is on the current commit, its build metadata will be used. If the tag is on an older commit, its build metadata will be ignored. Build metadata in `MinVerBuildMetadata` will be appended to build metadata in the tag.

### Can I use the version calculated by MinVer for other purposes?

Yes! You can use any of the [properties set by MinVer](#version-numbers), or override their values, in a target which runs after MinVer.

For example, for pull requests, you may want to inject the pull request number into the version. E.g. using Appveyor:

```xml
<Target Name="MyTarget" AfterTargets="MinVer" Condition="'$(APPVEYOR_PULL_REQUEST_NUMBER)' != ''" >
  <PropertyGroup>
    <PackageVersion>$(MinVerMajor).$(MinVerMinor).$(MinVerPatch)-PR.$(APPVEYOR_PULL_REQUEST_NUMBER).$(MinVerPreRelease)</PackageVersion>
    <PackageVersion Condition="'$(MinVerBuildMetadata)' != ''">$(PackageVersion)+$(MinVerBuildMetadata)</PackageVersion>
    <Version>$(PackageVersion)</Version>
  </PropertyGroup>
</Target>
```

Or for projects which do not create NuGet packages, you may want to populate [all four parts](https://docs.microsoft.com/en-us/dotnet/framework/app-domains/assembly-versioning#assembly-version-number) of `AssemblyVersion`. E.g. using Appveyor:

```xml
<Target Name="MyTarget" AfterTargets="MinVer">
  <PropertyGroup>
    <APPVEYOR_BUILD_NUMBER Condition="'$(APPVEYOR_BUILD_NUMBER)' == ''">0</APPVEYOR_BUILD_NUMBER>
    <AssemblyVersion>$(MinVerMajor).$(MinVerMinor).$(APPVEYOR_BUILD_NUMBER).$(MinVerPatch)</AssemblyVersion>
  </PropertyGroup>
</Target>
```

Or  for projects which _do_ create NuGet packages, you may want to adjust the assembly file version to include the build number, as recommended in the [official guidance](https://docs.microsoft.com/en-ca/dotnet/standard/library-guidance/versioning#assembly-file-version). E.g. when using Appveyor:

```xml
<Target Name="MyTarget" AfterTargets="MinVer">
  <PropertyGroup>
    <APPVEYOR_BUILD_NUMBER Condition="'$(APPVEYOR_BUILD_NUMBER)' == ''">0</APPVEYOR_BUILD_NUMBER>
    <FileVersion>$(MinVerMajor).$(MinVerMinor).$(MinVerPatch).$(APPVEYOR_BUILD_NUMBER)</FileVersion>
  </PropertyGroup>
</Target>
```

### Can I control the logging verbosity?

Yes! Set [`MinVerVerbosity`](#options) to `quiet`, `minimal`, `normal` (default), `detailed`, or `diagnostic`. At the `quiet` and `minimal` levels, you will see only warnings and errors. At the `detailed` level you will see how many commits were examined, which version tags were found but ignored, which version was calculated, etc. At the `diagnostic` level you will see how MinVer walks the commit history, in excruciating detail.

The verbosity levels reflect those supported by MSBuild and therefore `dotnet build`, `dotnet pack`, etc. In a future version of MinVer, these verbosity levels will be inherited from MSBuild and `MinVerVerbosity` will be deprecated. Currently this is not possible due to technical restrictions related to [libgit2](https://github.com/libgit2/libgit2).

### Can I use MinVer to version software which is not built using a .NET SDK style project?

Yes! MinVer is also available as a [command line tool](https://www.nuget.org/packages/minver-cli). Run `minver --help` for usage. The calculated version is printed to standard output (stdout).

### What if the history diverges, and more than one tag is found?

The tag with the higher version is used.

### What if the history diverges, and then converges again, before the latest tag (or root commit) is found?

MinVer will use the height on the first path followed where the history diverges. The paths are followed in the same order that the parents of the commit are stored in git. The first parent is the commit on the branch that was the current branch when the merge was performed. The remaining parents are stored in the order that their branches were specified in the merge command.

### Why does MinVer fail with `LibGit2Sharp.NotFoundException`?

You may see an exception of this form:

> Unhandled Exception: LibGit2Sharp.NotFoundException: object not found - no match for id (...)

This is because you are using a [shallow clone](https://www.git-scm.com/docs/git-clone#git-clone---depthltdepthgt). MinVer uses [libgit2](https://github.com/libgit2/libgit2) to interrogate the repo and [libgit2 does not support shallow clones](https://github.com/libgit2/libgit2/issues/3058). To resolve this problem, use a regular (deep) clone.

**Important:** By default, [Travis CI](https://travis-ci.org/) uses shallow clones with a depth of 50 commits. To build on Travis CI, [remove the `--depth` flag](https://docs.travis-ci.com/user/customizing-the-build#git-clone-depth).

### Why does MinVer fail with `System.TypeInitializationException`?

You may see an exception of this form:

> Unhandled Exception: System.TypeInitializationException: The type initializer for 'LibGit2Sharp.Core.NativeMethods' threw an exception. ---> System.DllNotFoundException: Unable to load shared library 'git2-8e0b172' or one of its dependencies.

This is probably because you are running on Linux, and you do not have libcurl installed. See the [prerequisites](#prerequisites).

---

<sub>[Tag](https://thenounproject.com/term/tag/938952) by [Ananth](https://thenounproject.com/ananthshas/) from [the Noun Project](https://thenounproject.com/).</sub>
