//
// ExposureNotifications C# bindings
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright (c) Microsoft Corporation.
//

using System;

using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace ExposureNotifications {

	[Introduced (PlatformName.iOS, 13, 5)]
	[Native]
	public enum ENErrorCode : long {
		Ok = 0,
		Unknown = 1,
		BadParameter = 2,
		NotEntitled = 3,
		NotAuthorized = 4,
		Unsupported = 5,
		Invalidated = 6,
		BluetoothOff = 7,
		InsufficientStorage = 8,
		NotEnabled = 9,
		ApiMisuse = 10,
		Internal = 11,
		InsufficientMemory = 12,
		RateLimited = 13,
		Restricted = 14,
		BadFormat = 15,
		DataInaccessible = 16,
		TravelStatusNotAvailable = 17,
	}

	[Introduced (PlatformName.iOS, 13, 5)]
	[Native]
	public enum ENAuthorizationStatus : long {
		Unknown = 0,
		Restricted = 1,
		NotAuthorized = 2,
		Authorized = 3,
	}

	[Introduced (PlatformName.iOS, 13, 5)]
	[Native]
	public enum ENStatus : long {
		Unknown = 0,
		Active = 1,
		Disabled = 2,
		BluetoothOff = 3,
		Restricted = 4,
		Paused = 5,
		Unauthorized = 6,
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	public enum ENDiagnosisReportType : uint {
		Unknown = 0,
		ConfirmedTest = 1,
		ConfirmedClinicalDiagnosis = 2,
		SelfReported = 3,
		Recursive = 4,
		Revoked = 5,
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	public enum ENCalibrationConfidence : byte {
		Lowest = 0,
		Low = 1,
		Medium = 2,
		High = 3,
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	public enum ENInfectiousness : uint {
		None = 0,
		Standard = 1,
		High = 2
	}

	delegate void ENErrorHandler ([NullAllowed] NSError error);

	[Introduced (PlatformName.iOS, 13, 5)]
	[BaseType (typeof (NSObject))]
	interface ENTemporaryExposureKey {

		[Export ("keyData", ArgumentSemantic.Copy)]
		NSData KeyData { get; set; }

		[Export ("rollingPeriod")]
		uint RollingPeriod { get; set; }

		[Export ("rollingStartNumber")]
		uint RollingStartNumber { get; set; }

		[Export ("transmissionRiskLevel")]
		byte TransmissionRiskLevel { get; set; }
	}

	[Introduced (PlatformName.iOS, 13, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ENExposureDetectionSummary {

		[BindAs (typeof (int []))]
		[Export ("attenuationDurations", ArgumentSemantic.Copy)]
		NSNumber [] AttenuationDurations { get; }

		[Export ("daysSinceLastExposure")]
		nint DaysSinceLastExposure { get; }

		[Export ("matchedKeyCount")]
		ulong MatchedKeyCount { get; }

		[Export ("maximumRiskScore")]
		byte MaximumRiskScore { get; }

		[Introduced (PlatformName.iOS, 13, 6)]
		[Export ("maximumRiskScoreFullRange")]
		double MaximumRiskScoreFullRange { get; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary Metadata { get; }

		[Introduced (PlatformName.iOS, 13, 6)]
		[Export ("riskScoreSumFullRange")]
		double RiskScoreSumFullRange { get; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("daySummaries", ArgumentSemantic.Copy)]
		ENExposureDaySummary [] DaySummaries { get; }
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ENExposureSummaryItem {

		[Export ("maximumScore")]
		double MaximumScore { get; }

		[Export ("scoreSum")]
		double ScoreSum { get; }

		[Export ("weightedDurationSum")]
		double WeightedDurationSum { get; }
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ENExposureWindow {

		[Export ("calibrationConfidence", ArgumentSemantic.Assign)]
		ENCalibrationConfidence CalibrationConfidence { get; }

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[Export ("diagnosisReportType", ArgumentSemantic.Assign)]
		ENDiagnosisReportType DiagnosisReportType { get; }

		[Export ("infectiousness", ArgumentSemantic.Assign)]
		ENInfectiousness Infectiousness { get; }

		[Export ("scanInstances", ArgumentSemantic.Copy)]
		ENScanInstance [] ScanInstances { get; }
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ENScanInstance {

		[Export ("minimumAttenuation")]
		byte MinimumAttenuation { get; }

		[Export ("typicalAttenuation")]
		byte TypicalAttenuation { get; }

		[Export ("secondsSinceLastScan")]
		nint SecondsSinceLastScan { get; }
	}

	[Introduced (PlatformName.iOS, 13, 5)]
	[BaseType (typeof (NSObject))]
	interface ENExposureConfiguration {

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("immediateDurationWeight")]
		double ImmediateDurationWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("nearDurationWeight")]
		double NearDurationWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("mediumDurationWeight")]
		double MediumDurationWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("otherDurationWeight")]
		double OtherDurationWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[NullAllowed, Export ("infectiousnessForDaysSinceOnsetOfSymptoms", ArgumentSemantic.Copy)]
		NSDictionary<NSNumber, NSNumber> InfectiousnessForDaysSinceOnsetOfSymptoms { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("infectiousnessStandardWeight")]
		double InfectiousnessStandardWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("infectiousnessHighWeight")]
		double InfectiousnessHighWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("reportTypeConfirmedTestWeight")]
		double ReportTypeConfirmedTestWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("reportTypeConfirmedClinicalDiagnosisWeight")]
		double ReportTypeConfirmedClinicalDiagnosisWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("reportTypeSelfReportedWeight")]
		double ReportTypeSelfReportedWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("reportTypeRecursiveWeight")]
		double ReportTypeRecursiveWeight { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("reportTypeNoneMap", ArgumentSemantic.Assign)]
		ENDiagnosisReportType ReportTypeNoneMap { get; set; }

		[Introduced (PlatformName.iOS, 13, 6)]
		[BindAs (typeof (int []))]
		[Export ("attenuationDurationThresholds", ArgumentSemantic.Copy)]
		NSNumber [] AttenuationDurationThresholds { get; set; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("daysSinceLastExposureThreshold")]
		nint DaysSinceLastExposureThreshold { get; set; }

		[Export ("minimumRiskScoreFullRange")]
		double MinimumRiskScoreFullRange { get; set; }

		[BindAs (typeof (int []))]
		[Export ("attenuationLevelValues", ArgumentSemantic.Copy)]
		NSNumber [] AttenuationLevelValues { get; set; }

		[Export ("attenuationWeight")]
		double AttenuationWeight { get; set; }

		[BindAs (typeof (int []))]
		[Export ("daysSinceLastExposureLevelValues", ArgumentSemantic.Copy)]
		NSNumber [] DaysSinceLastExposureLevelValues { get; set; }

		[Export ("daysSinceLastExposureWeight")]
		double DaysSinceLastExposureWeight { get; set; }

		[BindAs (typeof (int []))]
		[Export ("durationLevelValues", ArgumentSemantic.Copy)]
		NSNumber [] DurationLevelValues { get; set; }

		[Export ("durationWeight")]
		double DurationWeight { get; set; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary Metadata { get; set; }

		[Export ("minimumRiskScore")]
		byte MinimumRiskScore { get; set; }

		[BindAs (typeof (int []))]
		[Export ("transmissionRiskLevelValues", ArgumentSemantic.Copy)]
		NSNumber [] TransmissionRiskLevelValues { get; set; }

		[Export ("transmissionRiskWeight")]
		double TransmissionRiskWeight { get; set; }
	}

	[Introduced (PlatformName.iOS, 13, 7)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ENExposureDaySummary {

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[NullAllowed, Export ("confirmedTestSummary", ArgumentSemantic.Strong)]
		ENExposureSummaryItem ConfirmedTestSummary { get; }

		[NullAllowed, Export ("confirmedClinicalDiagnosisSummary", ArgumentSemantic.Strong)]
		ENExposureSummaryItem ConfirmedClinicalDiagnosisSummary { get; }

		[NullAllowed, Export ("recursiveSummary", ArgumentSemantic.Strong)]
		ENExposureSummaryItem RecursiveSummary { get; }

		[NullAllowed, Export ("selfReportedSummary", ArgumentSemantic.Strong)]
		ENExposureSummaryItem SelfReportedSummary { get; }

		[Export ("daySummary", ArgumentSemantic.Strong)]
		ENExposureSummaryItem DaySummary { get; }
	}

	[Introduced (PlatformName.iOS, 13, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ENExposureInfo {

		[BindAs (typeof (int []))]
		[Export ("attenuationDurations", ArgumentSemantic.Copy)]
		NSNumber [] AttenuationDurations { get; }

		[Export ("attenuationValue")]
		byte AttenuationValue { get; }

		[Export ("date", ArgumentSemantic.Copy)]
		NSDate Date { get; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("daysSinceOnsetOfSymptoms")]
		nint DaysSinceOnsetOfSymptoms { get; }

		[Introduced (PlatformName.iOS, 13, 7)]
		[Export ("diagnosisReportType", ArgumentSemantic.Assign)]
		ENDiagnosisReportType DiagnosisReportType { get; }

		[Export ("duration")]
		double Duration { get; }

		[NullAllowed, Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary Metadata { get; }

		[Export ("totalRiskScore")]
		byte TotalRiskScore { get; }

		[Introduced (PlatformName.iOS, 13, 6)]
		[Export ("totalRiskScoreFullRange")]
		double TotalRiskScoreFullRange { get; }

		[Export ("transmissionRiskLevel")]
		byte TransmissionRiskLevel { get; }
	}

	delegate void ENGetDiagnosisKeysHandler ([NullAllowed] ENTemporaryExposureKey [] keys, [NullAllowed] NSError error);
	delegate void ENDetectExposuresHandler ([NullAllowed] ENExposureDetectionSummary summary, [NullAllowed] NSError error);
	delegate void ENGetExposureInfoHandler ([NullAllowed] ENExposureInfo [] exposures, [NullAllowed] NSError error);
	delegate void ENGetExposureWindowsHandler ([NullAllowed] ENExposureWindow [] exposureWindows, [NullAllowed] NSError error);
	delegate void ENGetUserTraveledHandler (bool traveled, NSError error);
	delegate void ENDiagnosisKeysAvailableHandler ([NullAllowed] ENTemporaryExposureKey [] keys);

	[Introduced (PlatformName.iOS, 13, 5)]
	[BaseType (typeof (NSObject))]
	interface ENManager {

		[Export ("dispatchQueue", ArgumentSemantic.Strong)]
		DispatchQueue DispatchQueue { get; set; }

		[Export ("exposureNotificationStatus", ArgumentSemantic.Assign)]
		ENStatus ExposureNotificationStatus { get; }

		[NullAllowed, Export("invalidationHandler", ArgumentSemantic.Copy)]
		Action InvalidationHandler { get; set; }

		[Async]
		[Export ("activateWithCompletionHandler:")]
		void Activate (ENErrorHandler completionHandler);

		[Export ("invalidate")]
		void Invalidate ();

		[Introduced (PlatformName.iOS, 13, 7)]
		[Async]
		[Export ("getUserTraveledWithCompletionHandler:")]
		void GetUserTraveled (ENGetUserTraveledHandler completionHandler);

		[Static]
		[Export ("authorizationStatus", ArgumentSemantic.Assign)]
		ENAuthorizationStatus AuthorizationStatus { get; }

		[Export ("exposureNotificationEnabled")]
		bool ExposureNotificationEnabled { get; }

		[Async]
		[Export ("setExposureNotificationEnabled:completionHandler:")]
		void SetExposureNotificationEnabled (bool enabled, ENErrorHandler completionHandler);

		[Introduced (PlatformName.iOS, 13, 7)]
		[Async]
		[Export ("detectExposuresWithConfiguration:completionHandler:")]
		NSProgress DetectExposures (ENExposureConfiguration configuration, ENDetectExposuresHandler completionHandler);

		[Async]
		[Export ("detectExposuresWithConfiguration:diagnosisKeyURLs:completionHandler:")]
		NSProgress DetectExposures (ENExposureConfiguration configuration, NSUrl [] diagnosisKeyUrls, ENDetectExposuresHandler completionHandler);

		[Deprecated (PlatformName.iOS, 13, 6, message: "Use 'GetExposureWindows' instead.")]
		[Async]
		[Export ("getExposureInfoFromSummary:userExplanation:completionHandler:")]
		NSProgress GetExposureInfo (ENExposureDetectionSummary summary, string userExplanation, ENGetExposureInfoHandler completionHandler);

		[Introduced (PlatformName.iOS, 13, 7)]
		[Async]
		[Export ("getExposureWindowsFromSummary:completionHandler:")]
		NSProgress GetExposureWindows (ENExposureDetectionSummary summary, ENGetExposureWindowsHandler completionHandler);

		[Async]
		[Export ("getDiagnosisKeysWithCompletionHandler:")]
		void GetDiagnosisKeys (ENGetDiagnosisKeysHandler completionHandler);

		[Async]
		[Export ("getTestDiagnosisKeysWithCompletionHandler:")]
		void GetTestDiagnosisKeys (ENGetDiagnosisKeysHandler completionHandler);

		[Introduced (PlatformName.iOS, 14, 4)]
		[NullAllowed, Export ("diagnosisKeysAvailableHandler", ArgumentSemantic.Copy)]
		ENDiagnosisKeysAvailableHandler DiagnosisKeysAvailableHandler { get; set; }

		[Introduced (PlatformName.iOS, 14, 4)]
		[Async]
		[Export ("preAuthorizeDiagnosisKeysWithCompletionHandler:")]
		void PreAuthorizeDiagnosisKeys (ENErrorHandler completionHandler);

		[Introduced (PlatformName.iOS, 14, 4)]
		[Async]
		[Export ("requestPreAuthorizedDiagnosisKeysWithCompletionHandler:")]
		void RequestPreAuthorizedDiagnosisKeys (ENErrorHandler completionHandler);
	}
}
