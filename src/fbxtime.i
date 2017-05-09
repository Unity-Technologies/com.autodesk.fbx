// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * This file handles both FbxTime and FbxTimeSpan since they're in the same
 * header file.
 */

/***************************************************************************
 * FbxTime
 ***************************************************************************/

%rename("%s", %$isclass) FbxTime;
%rename("%s") FbxTime::FbxTime(); /* but not the constructor with arguments, see the 'From' functions below */
%rename("%s") FbxTime::~FbxTime;
%rename("%s") FbxTime::EElement;
%rename("%s") FbxTime::EMode;
%rename("%s") FbxTime::EProtocol;


%define_equality_from_operator(FbxTime);
%extend FbxTime { %proxycode %{
  public override int GetHashCode() { return GetRaw().GetHashCode(); }
%} }

/*
 * https://developer.blender.org/T48610
 *
 * If you set a custom global frame rate, it becomes 12.5. This is
 * actually how it's documented in earlier versions of FBX!
 *    "in case of timemode = custom, we specify the custom framerate to use: EX:12.5"
 *
 * Worse, you get a frame rate of 12.5 by default from then on, even if you
 * switch to PAL or NTSC or whatever.
 *
 * So don't let users set a custom frame rate.
 *
 * The other static functions are fine.
 */
%rename("%s") FbxTime::SetGlobalTimeMode(EMode);
%rename("%s") FbxTime::GetGlobalTimeMode;
%rename("%s") FbxTime::SetGlobalTimeProtocol;
%rename("%s") FbxTime::GetGlobalTimeProtocol;
%rename("%s") FbxTime::GetFrameRate;
%rename("%s") FbxTime::ConvertFrameRateToTimeMode;
%rename("%s") FbxTime::IsDropFrame;
%rename("%s") FbxTime::GetOneFrameValue;

/*
 * Make FbxTime into an immutable type, to act more like C# generally acts.
 * This also makes it easier to optimize (todo!)
 *
 * Also ignore the constructor from 64-bit to avoid bugs with people assuming
 * the argument is frames or seconds (it's not).
 *
 * Instead, for every 'Set' function in C++ there's a corresponding
 * 'From' C# function.
 */
%extend FbxTime {
  /* Matches the constructor, and the Set function */
  %ignore FbxTime(const FbxLongLong);
  %ignore Set;
  %rename("%s") FromRaw;
  static FbxTime FromRaw(FbxLongLong pTime) {
    return FbxTime(pTime);
  }

  /* Matches SetMilliSeconds. */
  %ignore SetMilliSeconds;
  %rename("%s") FromMilliSeconds;
  static FbxTime FromMilliSeconds(FbxLongLong pMilliSeconds) {
    FbxTime t;
    t.SetMilliSeconds(pMilliSeconds);
    return t;
  }

  /* Matches SetSecondDouble and FbxTimeSeconds. */
  %ignore SetSecondDouble;
  %rename("%s") FromSecondDouble;
  static FbxTime FromSecondDouble(double pTime) {
    FbxTime t;
    t.SetSecondDouble(pTime);
    return t;
  }

  /* Match FromTime, and allow optional arguments. SWIG can't handle that natively,
   * so we help it. */
  %ignore SetTime;
  %csmethodmodifiers FromTimeNoOptionalArgs "private";
  %rename("%s") FromTimeNoOptionalArgs;
  static FbxTime FromTimeNoOptionalArgs(int pHour, int pMinute, int pSecond, int pFrame, int pField, int pResidual, EMode pTimeMode) {
    FbxTime t;
    t.SetTime(pHour, pMinute, pSecond, pFrame, pField, pResidual, pTimeMode);
    return t;
  }
  %proxycode %{
  public static FbxTime FromTime(int pHour, int pMinute, int pSecond, int pFrame=0, int pField=0, int pResidual=0, EMode pTimeMode=EMode.eDefaultMode) {
    return FromTimeNoOptionalArgs(pHour, pMinute, pSecond, pFrame, pField, pResidual, pTimeMode);
  }
  %}

  /* Matches SetFrame */
  %ignore SetFrame;
  %rename("%s") FromFrame;
  static FbxTime FromFrame(long long pFrames, EMode pTimeMode=eDefaultMode) {
    FbxTime t;
    t.SetFrame(pFrames, pTimeMode);
    return t;
  }

  /* Matches SetFramePrecise */
  %ignore SetFramePrecise;
  %rename("%s") FromFramePrecise;
  static FbxTime FromFramePrecise(double pFrames, EMode pTimeMode=eDefaultMode) {
    FbxTime t;
    t.SetFramePrecise(pFrames, pTimeMode);
    return t;
  }

  /* Matches SetTimeString. Allow setting only the time format via optional args. */
  %ignore SetTimeString;
  %rename("%s") FromTimeStringNoOptionalArgs;
  %csmethodmodifiers FromTimeStringNoOptionalArgs "private";
  static FbxTime FromTimeStringNoOptionalArgs(const char* pTime, EMode pTimeMode, EProtocol pTimeFormat) {
    FbxTime t;
    t.SetTimeString(pTime, pTimeMode, pTimeFormat);
    return t;
  }
  %proxycode %{
  public static FbxTime FromTimeString(string pTime, EMode pTimeMode=EMode.eDefaultMode, EProtocol pTimeFormat=EProtocol.eDefaultProtocol) {
    return FbxTime.FromTimeStringNoOptionalArgs(pTime, pTimeMode, pTimeFormat);
  }
  %}
}

/* Getters */
%rename("GetRaw") FbxTime::Get;
%rename("%s") FbxTime::GetMilliSeconds;
%rename("%s") FbxTime::GetSecondDouble;
%rename("%s") FbxTime::GetHourCount;
%rename("%s") FbxTime::GetMinuteCount;
%rename("%s") FbxTime::GetSecondCount;
%rename("%s") FbxTime::GetFrameCount;
%rename("%s") FbxTime::GetFrameCountPrecise;
%rename("%s") FbxTime::GetFieldCount;
%rename("%s") FbxTime::GetResidual;
%rename("%s") FbxTime::GetFrameSeparator;

%rename("%s") FbxTime::GetFramedTime;

/* GetTime returns a bunch of int by ref. */
%apply int& OUTPUT { int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField, int& pResidual };
%rename("%s") FbxTime::GetTime;

/* GetTimeString has two variants. Ignore the one that takes in a buffer, use
 * the one that returns an FbxString. It has lots of arguments; use C# default
 * arguments. */
%csmethodmodifiers FbxTime::GetTimeString "private";
%ignore FbxTime::GetTimeString;
%rename("GetTimeStringNoOptionalArgs") FbxTime::GetTimeString(EElement pStart, EElement pEnd, EMode pTimeMode, EProtocol pTimeFormat) const;
%extend FbxTime {
  %proxycode %{
    public string GetTimeString(
        EElement pStart = EElement.eHours,
        EElement pEnd = EElement.eResidual,
        EMode pTimeMode = EMode.eDefaultMode,
        EProtocol pTimeFormat = EProtocol.eDefaultProtocol)
    {
      return GetTimeStringNoOptionalArgs(pStart, pEnd, pTimeMode, pTimeFormat);
    }
  %}
}

/***************************************************************************
 * FbxTimeSpan
 ***************************************************************************/

%rename("%s", %$isclass) FbxTimeSpan;
%rename("%s") FbxTimeSpan::FbxTimeSpan;
%rename("%s") FbxTimeSpan::~FbxTimeSpan;
%define_equality_from_operator(FbxTimeSpan);
%extend FbxTimeSpan { %proxycode %{
  public override int GetHashCode() {
    long a = GetStart().GetRaw();
    long b = GetStop().GetRaw();
    int hash = (int)a;
    hash = (hash << 9) | (hash >> 23);
    hash ^= (int)(a >> 32);
    hash = (hash << 9) | (hash >> 23);
    hash ^= (int)b;
    hash = (hash << 9) | (hash >> 23);
    hash ^= (int)(b >> 32);
    return hash;
  }
%} }

%rename("%s") FbxTimeSpan::Set;
%rename("%s") FbxTimeSpan::SetStart;
%rename("%s") FbxTimeSpan::SetStop;
%rename("%s") FbxTimeSpan::GetStart;
%rename("%s") FbxTimeSpan::GetStop;
%rename("%s") FbxTimeSpan::GetDuration;
%rename("%s") FbxTimeSpan::GetSignedDuration;
%rename("%s") FbxTimeSpan::GetDirection;
%rename("%s") FbxTimeSpan::IsInside;
%rename("%s") FbxTimeSpan::Intersect;
%rename("%s") FbxTimeSpan::UnionAssignment;

/***************************************************************************
 * The include
 ***************************************************************************/
%include "fbxsdk/core/base/fbxtime.h"
