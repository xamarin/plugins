using System;

#if __UNIFIED__
using CoreGraphics;
using Foundation;
using MessageUI;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using CGRect = System.Drawing.RectangleF;
using nint = System.Int32;
using nfloat = System.Single;
#endif

namespace InAppSettingsKit
{
	interface ISettingsStore
	{
	}

	// @protocol IASKSettingsStore <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject), Name = "IASKSettingsStore")]
	interface SettingsStore
	{
		// @required -(void)setBool:(BOOL)value forKey:(NSString *)key;
		[Abstract]
		[Export ("setBool:forKey:")]
		void SetBool (bool value, string key);

		// @required -(void)setFloat:(float)value forKey:(NSString *)key;
		[Abstract]
		[Export ("setFloat:forKey:")]
		void SetFloat (float value, string key);

		// @required -(void)setDouble:(double)value forKey:(NSString *)key;
		[Abstract]
		[Export ("setDouble:forKey:")]
		void SetDouble (double value, string key);

		// @required -(void)setInteger:(NSInteger)value forKey:(NSString *)key;
		[Abstract]
		[Export ("setInteger:forKey:")]
		void SetInteger (nint value, string key);

		// @required -(void)setObject:(id)value forKey:(NSString *)key;
		[Abstract]
		[Export ("setObject:forKey:")]
		void SetObject (NSObject value, string key);

		// @required -(BOOL)boolForKey:(NSString *)key;
		[Abstract]
		[Export ("boolForKey:")]
		bool GetBool (string key);

		// @required -(float)floatForKey:(NSString *)key;
		[Abstract]
		[Export ("floatForKey:")]
		float GetFloat (string key);

		// @required -(double)doubleForKey:(NSString *)key;
		[Abstract]
		[Export ("doubleForKey:")]
		double GetDouble (string key);

		// @required -(NSInteger)integerForKey:(NSString *)key;
		[Abstract]
		[Export ("integerForKey:")]
		nint GetInteger (string key);

		// @required -(id)objectForKey:(NSString *)key;
		[Abstract]
		[Export ("objectForKey:")]
		NSObject GetObject (string key);

		// @required -(BOOL)synchronize;
		[Abstract]
		[Export ("synchronize")]
		bool Synchronize ();
	}

	// @interface IASKAbstractSettingsStore : NSObject <IASKSettingsStore>
	[BaseType (typeof(NSObject), Name = "IASKAbstractSettingsStore")]
	interface AbstractSettingsStore : SettingsStore
	{
		// -(void)setObject:(id)value forKey:(NSString *)key;
		[Abstract]
		[Export ("setObject:forKey:")]
		void SetObject (NSObject value, string key);

		// -(id)objectForKey:(NSString *)key;
		[Abstract]
		[Export ("objectForKey:")]
		NSObject GetObject (string key);

		// -(BOOL)synchronize;
		[Export ("synchronize")]
		bool Synchronize ();
	}

	interface ISettingsViewController
	{
	}

	// @protocol IASKViewController <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject), Name = "IASKViewController")]
	interface SettingsViewController
	{
		// @required @property (retain, nonatomic) IASKSettingsReader * settingsReader;
		[Export ("settingsReader", ArgumentSemantic.Retain)]
		SettingsReader SettingsReader { get; set; }

		// @required @property (retain, nonatomic) id<IASKSettingsStore> settingsStore;
		[Export ("settingsStore", ArgumentSemantic.Retain)]
		ISettingsStore SettingsStore { get; set; }
	}

	// @interface IASKSpecifier : NSObject
	[BaseType (typeof(NSObject), Name = "IASKSpecifier")]
	interface SettingsSpecifier
	{
		// @property (retain, nonatomic) NSDictionary * specifierDict;
		[Export ("specifierDict", ArgumentSemantic.Retain)]
		NSDictionary SpecifierDict { get; set; }

		// @property (nonatomic, weak) IASKSettingsReader * _Nullable settingsReader;
		[NullAllowed, Export ("settingsReader", ArgumentSemantic.Weak)]
		SettingsReader SettingsReader { get; set; }

		// -(id)initWithSpecifier:(NSDictionary *)specifier;
		[Export ("initWithSpecifier:")]
		IntPtr Constructor (NSDictionary specifier);

		// -(id)initWithSpecifier:(NSDictionary *)specifier radioGroupValue:(NSString *)radioGroupValue;
		[Export ("initWithSpecifier:radioGroupValue:")]
		IntPtr Constructor (NSDictionary specifier, string radioGroupValue);

        // -(void)setMultipleValuesDictValues:(NSArray *)values titles:(NSArray *)titles;
        [Export("setMultipleValuesDictValues:titles:")]
        void SetMultipleValuesDictValues(NSObject[] values, NSObject[] titles);

        // -(void)sortIfNeeded;
        [Export ("sortIfNeeded")]
		void SortIfNeeded ();

		// -(NSString *)localizedObjectForKey:(NSString *)key;
		[Export ("localizedObjectForKey:")]
		string GetLocalizedObject (string key);

		// -(NSString *)title;
		[Export ("title")]
		string Title { get; }

		// -(NSString *)subtitle;
		[Export ("subtitle")]
		string Subtitle { get; }

        // -(NSString *)placeholder;
        [Export("placeholder")]
        string Placeholder { get; }

        // -(NSString *)key;
        [Export ("key")]
		string Key { get; }

		// -(NSString *)type;
		[Export ("type")]
		string Type { get; }

		// -(NSString *)titleForCurrentValue:(id)currentValue;
		[Export ("titleForCurrentValue:")]
		string GetTitle (NSObject currentValue);

		// -(NSInteger)multipleValuesCount;
		[Export ("multipleValuesCount")]
		nint MultipleValuesCount { get; }

		// -(NSArray *)multipleValues;
		[Export ("multipleValues")]
		NSObject[] MultipleValues { get; }

		// -(NSArray *)multipleTitles;
		[Export ("multipleTitles")]
		NSObject[] MultipleTitles { get; }

        // -(NSArray *)multipleIconNames;
        [Export("multipleIconNames")]
        NSObject[] MultipleIconNames { get; }

        // -(NSString *)file;
        [Export ("file")]
		string File { get; }

		// -(id)defaultValue;
		[Export ("defaultValue")]
		NSObject DefaultValue { get; }

		// -(id)defaultStringValue;
		[Export ("defaultStringValue")]
		NSString DefaultStringValue { get; }

		// -(BOOL)defaultBoolValue;
		[Export ("defaultBoolValue")]
		bool DefaultBoolValue { get; }

		// -(id)trueValue;
		[Export ("trueValue")]
		NSObject TrueValue { get; }

		// -(id)falseValue;
		[Export ("falseValue")]
		NSObject FalseValue { get; }

		// -(float)minimumValue;
		[Export ("minimumValue")]
		float MinimumValue { get; }

		// -(float)maximumValue;
		[Export ("maximumValue")]
		float MaximumValue { get; }

		// -(NSString *)minimumValueImage;
		[Export ("minimumValueImage")]
		string MinimumValueImage { get; }

		// -(NSString *)maximumValueImage;
		[Export ("maximumValueImage")]
		string MaximumValueImage { get; }

		// -(BOOL)isSecure;
		[Export ("isSecure")]
		bool IsSecure { get; }

		// -(BOOL)displaySortedByTitle;
		[Export ("displaySortedByTitle")]
		bool DisplaySortedByTitle { get; }

		// -(UIKeyboardType)keyboardType;
		[Export ("keyboardType")]
		UIKeyboardType KeyboardType { get; }

		// -(UITextAutocapitalizationType)autocapitalizationType;
		[Export ("autocapitalizationType")]
		UITextAutocapitalizationType AutocapitalizationType { get; }

		// -(UITextAutocorrectionType)autoCorrectionType;
		[Export ("autoCorrectionType")]
		UITextAutocorrectionType AutoCorrectionType { get; }

		// -(NSString *)footerText;
		[Export ("footerText")]
		string FooterText { get; }

		// -(Class)viewControllerClass;
		[Export ("viewControllerClass")]
		Class ViewControllerClass { get; }

		// -(SEL)viewControllerSelector;
		[Export ("viewControllerSelector")]
		Selector ViewControllerSelector { get; }

		// -(NSString *)viewControllerStoryBoardFile;
		[Export ("viewControllerStoryBoardFile")]
		string ViewControllerStoryBoardFile { get; }

		// -(NSString *)viewControllerStoryBoardID;
		[Export ("viewControllerStoryBoardID")]
		string ViewControllerStoryBoardId { get; }

        // -(NSString *)segueIdentifier;
        [Export("segueIdentifier")]
        string SegueIdentifier { get; }

        // -(Class)buttonClass;
        [Export ("buttonClass")]
		Class ButtonClass { get; }

		// -(SEL)buttonAction;
		[Export ("buttonAction")]
		Selector ButtonAction { get; }

		// -(UIImage *)cellImage;
		[Export ("cellImage")]
		UIImage CellImage { get; }

		// -(UIImage *)highlightedCellImage;
		[Export ("highlightedCellImage")]
		UIImage HighlightedCellImage { get; }

		// -(BOOL)adjustsFontSizeToFitWidth;
		[Export ("adjustsFontSizeToFitWidth")]
		bool AdjustsFontSizeToFitWidth { get; }

		// -(NSTextAlignment)textAlignment;
		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; }

		// -(NSArray *)userInterfaceIdioms;
		[Export ("userInterfaceIdioms")]
		NSObject[] GetUserInterfaceIdioms ();

		// -(NSString *)radioGroupValue;
		[Export ("radioGroupValue")]
		string RadioGroupValue { get; }
	}

	interface ISettingsDelegate
	{
	}

	// @protocol IASKSettingsDelegate
	[Protocol, Model]
	[BaseType (typeof(NSObject), Name = "IASKSettingsDelegate")]
	[DisableDefaultCtor]
	interface SettingsDelegate
	{
		// @required -(void)settingsViewControllerDidEnd:(IASKAppSettingsViewController *)sender;
		[Abstract]
		[Export ("settingsViewControllerDidEnd:")]
		void SettingsViewControllerDidEnd (AppSettingsViewController sender);

        // @optional -(NSString *)settingsViewController:(id<IASKViewController>)settingsViewController tableView:(UITableView *)tableView titleForHeaderForSection:(NSInteger)section;
        [Export("settingsViewController:tableView:titleForHeaderForSection:")]
        string GetTitleForHeaderForSection(ISettingsViewController settingsViewController, UITableView tableView, nint section);

        // @optional -(CGFloat)settingsViewController:(id<IASKViewController>)settingsViewController tableView:(UITableView *)tableView heightForHeaderForSection:(NSInteger)section;
        [Export ("settingsViewController:tableView:heightForHeaderForSection:")]
		nfloat GetHeightForHeaderForSection (ISettingsViewController settingsViewController, UITableView tableView, nint section);

		// @optional -(UIView *)settingsViewController:(id<IASKViewController>)settingsViewController tableView:(UITableView *)tableView viewForHeaderForSection:(NSInteger)section;
		[Export ("settingsViewController:tableView:viewForHeaderForSection:")]
		UIView GetViewForHeaderForSection (ISettingsViewController settingsViewController, UITableView tableView, nint section);

        // @optional -(NSString *)settingsViewController:(id<IASKViewController>)settingsViewController tableView:(UITableView *)tableView titleForFooterForSection:(NSInteger)section;
        [Export("settingsViewController:tableView:titleForFooterForSection:")]
        string GetTitleForFooterForSection(ISettingsViewController settingsViewController, UITableView tableView, nint section);

        // @optional -(CGFloat)settingsViewController:(id<IASKViewController>)settingsViewController tableView:(UITableView *)tableView heightForFooterForSection:(NSInteger)section;
        [Export("settingsViewController:tableView:heightForFooterForSection:")]
        nfloat GetHeightForFooterForSection(ISettingsViewController settingsViewController, UITableView tableView, nint section);

        // @optional -(UIView *)settingsViewController:(id<IASKViewController>)settingsViewController tableView:(UITableView *)tableView viewForFooterForSection:(NSInteger)section;
        [Export("settingsViewController:tableView:viewForFooterForSection:")]
        UIView GetViewForFooterForSection(ISettingsViewController settingsViewController, UITableView tableView, nint section);

        // @optional -(CGFloat)tableView:(UITableView *)tableView heightForSpecifier:(IASKSpecifier *)specifier;
        [Export ("tableView:heightForSpecifier:")]
		nfloat GetHeightForSpecifier (UITableView tableView, SettingsSpecifier specifier);

		// @optional -(UITableViewCell *)tableView:(UITableView *)tableView cellForSpecifier:(IASKSpecifier *)specifier;
		[Export ("tableView:cellForSpecifier:")]
		UITableViewCell GetCellForSpecifier (UITableView tableView, SettingsSpecifier specifier);

        // @optional -(BOOL)settingsViewController:(id<IASKViewController>)settingsViewController shouldPresentMailComposeViewController:(MFMailComposeViewController *)mailComposeViewController forSpecifier:(IASKSpecifier *)specifier;
        [Export("settingsViewController:shouldPresentMailComposeViewController:forSpecifier:")]
        bool ShouldPresentMailComposeViewController(ISettingsViewController settingsViewController, MFMailComposeViewController mailComposeViewController, SettingsSpecifier     specifier);

        // @optional -(NSString *)settingsViewController:(id<IASKViewController>)settingsViewController mailComposeBodyForSpecifier:(IASKSpecifier *)specifier;
        [Export ("settingsViewController:mailComposeBodyForSpecifier:")]
		string GetMailComposeBodyForSpecifier (ISettingsViewController settingsViewController, SettingsSpecifier specifier);

		// @optional -(UIViewController<MFMailComposeViewControllerDelegate> *)settingsViewController:(id<IASKViewController>)settingsViewController viewControllerForMailComposeViewForSpecifier:(IASKSpecifier *)specifier;
		[Export ("settingsViewController:viewControllerForMailComposeViewForSpecifier:")]
		MFMailComposeViewControllerDelegate GetViewControllerForMailComposeViewForSpecifier (ISettingsViewController settingsViewController, SettingsSpecifier specifier);

		// @optional -(void)settingsViewController:(id<IASKViewController>)settingsViewController mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error;
		[Export ("settingsViewController:mailComposeController:didFinishWithResult:error:")]
		void DidFinishWithResult (ISettingsViewController settingsViewController, MFMailComposeViewController controller, MFMailComposeResult result, NSError error);

        // @optional -(NSArray *)settingsViewController:(IASKAppSettingsViewController *)sender valuesForSpecifier:(IASKSpecifier *)specifier;
        [Export("settingsViewController:valuesForSpecifier:")]
        NSObject[] GetValuesForSpecifier(ISettingsViewController sender, SettingsSpecifier specifier);

        // @optional -(NSArray *)settingsViewController:(IASKAppSettingsViewController *)sender titlesForSpecifier:(IASKSpecifier *)specifier;
        [Export("settingsViewController:titlesForSpecifier:")]
        NSObject[] GetTitlesForSpecifier(ISettingsViewController sender, SettingsSpecifier specifier);

        // DEPRECATED
        //		// @optional -(void)settingsViewController:(IASKAppSettingsViewController *)sender buttonTappedForKey:(NSString *)key __attribute__((deprecated("")));
        //		[Export ("settingsViewController:buttonTappedForKey:")]
        //		void SettingsViewController (IASKAppSettingsViewController sender, string key);

        // @optional -(void)settingsViewController:(IASKAppSettingsViewController *)sender buttonTappedForSpecifier:(IASKSpecifier *)specifier;
        [Export ("settingsViewController:buttonTappedForSpecifier:")]
		void ButtonTappedForSpecifier (AppSettingsViewController sender, SettingsSpecifier specifier);

		// @optional -(void)settingsViewController:(IASKAppSettingsViewController *)sender tableView:(UITableView *)tableView didSelectCustomViewSpecifier:(IASKSpecifier *)specifier;
		[Export ("settingsViewController:tableView:didSelectCustomViewSpecifier:")]
		void DidSelectCustomViewSpecifier (AppSettingsViewController sender, UITableView tableView, SettingsSpecifier specifier);
	}

	// @interface IASKAppSettingsViewController : UITableViewController <IASKViewController, UITextFieldDelegate, MFMailComposeViewControllerDelegate>
	[BaseType (typeof(UITableViewController), Name = "IASKAppSettingsViewController")]
	interface AppSettingsViewController : SettingsViewController, IUITextFieldDelegate, IMFMailComposeViewControllerDelegate
	{
		// @property (assign, nonatomic) id delegate __attribute__((iboutlet));
		[NullAllowed, Export ("delegate", ArgumentSemantic.Assign)]
		ISettingsDelegate Delegate { get; set; }

		// @property (copy, nonatomic) NSString * file;
		[Export ("file")]
		string File { get; set; }

		// @property (assign, nonatomic) BOOL showCreditsFooter;
		[Export ("showCreditsFooter")]
		bool ShowCreditsFooter { get; set; }

		// @property (assign, nonatomic) BOOL showDoneButton;
		[Export ("showDoneButton")]
		bool ShowDoneButton { get; set; }

		// @property (retain, nonatomic) NSSet * hiddenKeys;
		[Export ("hiddenKeys", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSSet HiddenKeys { get; set; }

		// @property (nonatomic) BOOL neverShowPrivacySettings;
		[Export ("neverShowPrivacySettings")]
		bool NeverShowPrivacySettings { get; set; }

        // @property (nonatomic) BOOL cellLayoutMarginsFollowReadableWidth;
        [Export("cellLayoutMarginsFollowReadableWidth")]
        bool CellLayoutMarginsFollowReadableWidth { get; set; }

        // -(void)synchronizeSettings;
        [Export ("synchronizeSettings")]
		void SynchronizeSettings ();

		// -(void)dismiss:(id)sender;
		[Export ("dismiss:")]
		void Dismiss (NSObject sender);

		// -(void)setHiddenKeys:(NSSet *)hiddenKeys animated:(BOOL)animated;
		[Export ("setHiddenKeys:animated:")]
		void SetHiddenKeys ([NullAllowed] NSSet hiddenKeys, bool animated);
	}

	// @interface IASKAppSettingsWebViewController : UIViewController <UIWebViewDelegate, MFMailComposeViewControllerDelegate>
	[BaseType (typeof(UIViewController), Name = "IASKAppSettingsWebViewController")]
	interface AppSettingsWebViewController : IUIWebViewDelegate, IMFMailComposeViewControllerDelegate
	{
		// -(id)initWithFile:(NSString *)htmlFileName specifier:(IASKSpecifier *)specifier;
		[Export ("initWithFile:specifier:")]
		IntPtr Constructor (string htmlFileName, SettingsSpecifier specifier);

		// @property (nonatomic, strong) UIWebView * webView;
		[Export ("webView", ArgumentSemantic.Strong)]
		UIWebView WebView { get; set; }

		// @property (nonatomic, strong) NSURL * url;
		[Export ("url", ArgumentSemantic.Strong)]
		NSUrl Url { get; set; }

		// @property (nonatomic, strong) NSString * customTitle;
		[Export ("customTitle", ArgumentSemantic.Strong)]
		string CustomTitle { get; set; }
	}

	// @interface IASKMultipleValueSelection : NSObject
	[BaseType (typeof(NSObject), Name = "IASKMultipleValueSelection")]
	interface MultipleValueSelection
	{
		// @property (assign, nonatomic) UITableView * tableView;
		[Export ("tableView", ArgumentSemantic.Assign)]
		UITableView TableView { get; set; }

		// @property (retain, nonatomic) IASKSpecifier * specifier;
		[Export ("specifier", ArgumentSemantic.Retain)]
		SettingsSpecifier Specifier { get; set; }

		// @property (assign, nonatomic) NSInteger section;
		[Export ("section")]
		nint Section { get; set; }

		// @property (readonly, copy, nonatomic) NSIndexPath * checkedItem;
		[Export ("checkedItem", ArgumentSemantic.Copy)]
		NSIndexPath CheckedItem { get; }

		// @property (nonatomic, strong) id<IASKSettingsStore> settingsStore;
		[Export ("settingsStore", ArgumentSemantic.Strong)]
		ISettingsStore SettingsStore { get; set; }

        // -(id)initWithSettingsStore:(id<IASKSettingsStore>)settingsStore;
        [Export("initWithSettingsStore:")]
        IntPtr Constructor(ISettingsStore settingsStore);

        // -(void)selectRowAtIndexPath:(NSIndexPath *)indexPath;
        [Export ("selectRowAtIndexPath:")]
		void SelectRowAtIndexPath (NSIndexPath indexPath);

		// -(void)updateSelectionInCell:(UITableViewCell *)cell indexPath:(NSIndexPath *)indexPath;
		[Export ("updateSelectionInCell:indexPath:")]
		void UpdateSelectionInCell (UITableViewCell cell, NSIndexPath indexPath);
	}


    // @interface IASKSpecifierValuesViewController : UITableViewController <IASKViewController>
    [BaseType(typeof(UITableViewController), Name = "IASKSpecifierValuesViewController")]
    interface SpecifierValuesViewController : SettingsViewController
    {
        // @property (retain, nonatomic) IASKSpecifier * currentSpecifier;
        [Export("currentSpecifier", ArgumentSemantic.Retain)]
        SettingsSpecifier CurrentSpecifier { get; set; }
    }

    // @interface IASKSettingsReader : NSObject
    [BaseType(typeof(NSObject), Name = "IASKSettingsReader")]
    interface SettingsReader
    {
        // -(id)initWithSettingsFileNamed:(NSString *)fileName applicationBundle:(NSBundle *)bundle;
        [Export("initWithSettingsFileNamed:applicationBundle:")]
        IntPtr Constructor(string fileName, NSBundle bundle);

        // -(id)initWithFile:(NSString *)file;
        [Export("initWithFile:")]
        IntPtr Constructor(string file);

        // -(NSInteger)numberOfSections;
        [Export("numberOfSections")]
        nint NumberOfSections { get; }

        // -(NSInteger)numberOfRowsForSection:(NSInteger)section;
        [Export("numberOfRowsForSection:")]
        nint GetNumberOfRows(nint section);

        // -(IASKSpecifier *)specifierForIndexPath:(NSIndexPath *)indexPath;
        [Export("specifierForIndexPath:")]
        SettingsSpecifier GetSpecifier(NSIndexPath indexPath);

        // -(IASKSpecifier *)headerSpecifierForSection:(NSInteger)section;
        [Export("headerSpecifierForSection:")]
        SettingsSpecifier GetHeaderSpecifier(nint section);

        // -(NSIndexPath *)indexPathForKey:(NSString *)key;
        [Export("indexPathForKey:")]
        NSIndexPath GetIndexPath(string key);

        // -(IASKSpecifier *)specifierForKey:(NSString *)key;
        [Export("specifierForKey:")]
        SettingsSpecifier GetSpecifier(string key);

        // -(NSString *)titleForSection:(NSInteger)section;
        [Export("titleForSection:")]
        string GetTitle(nint section);

        // -(NSString *)keyForSection:(NSInteger)section;
        [Export("keyForSection:")]
        [return: NullAllowed]
        string GetKey(nint section);

        // -(NSString *)footerTextForSection:(NSInteger)section;
        [Export("footerTextForSection:")]
        string GetFooterText(nint section);

        // -(NSString *)titleForId:(NSObject *)titleId;
        [Export("titleForId:")]
        string GetTitle(string titleId);

        // -(NSString *)pathForImageNamed:(NSString *)image;
        [Export("pathForImageNamed:")]
        string GetPathForImage(string image);

        // @property (readonly, nonatomic) NSBundle * applicationBundle;
        [Export("applicationBundle")]
        NSBundle ApplicationBundle { get; }

        // @property (readonly, nonatomic) NSBundle * settingsBundle;
        [Export("settingsBundle")]
        NSBundle SettingsBundle { get; }

        // @property (readonly, nonatomic) NSDictionary * settingsDictionary;
        [Export("settingsDictionary")]
        NSDictionary SettingsDictionary { get; }

        // @property (retain, nonatomic) NSString * localizationTable;
        [Export("localizationTable", ArgumentSemantic.Retain)]
        string LocalizationTable { get; set; }

        // @property (retain, nonatomic) NSArray * dataSource;
        [Export("dataSource", ArgumentSemantic.Retain)]
        NSObject[] DataSource { get; set; }

        // @property (retain, nonatomic) NSSet * hiddenKeys;
        [Export("hiddenKeys", ArgumentSemantic.Retain)]
        [NullAllowed]
        NSSet HiddenKeys { get; set; }

        // @property (nonatomic) BOOL showPrivacySettings;
        [Export("showPrivacySettings")]
        bool ShowPrivacySettings { get; set; }

        // -(NSString *)file:(NSString *)file withBundle:(NSString *)bundle suffix:(NSString *)suffix extension:(NSString *)extension;
        [Export("file:withBundle:suffix:extension:")]
        string GetFile(string file, string bundle, string suffix, string extension);

        // -(NSString *)locateSettingsFile:(NSString *)file;
        [Export("locateSettingsFile:")]
        string LocateSettingsFile(string file);

        // -(NSString *)platformSuffixForInterfaceIdiom:(UIUserInterfaceIdiom)interfaceIdiom;
        [Export("platformSuffixForInterfaceIdiom:")]
        string GetPlatformSuffix(UIUserInterfaceIdiom interfaceIdiom);
    }

    // @interface IASKSettingsStoreFile : IASKAbstractSettingsStore
    [BaseType(typeof(AbstractSettingsStore), Name = "IASKSettingsStoreFile")]
    interface SettingsStoreFile
    {
        // -(id)initWithPath:(NSString *)path;
        [Export("initWithPath:")]
        IntPtr Constructor(string path);

        // @property (readonly, copy, nonatomic) NSString * filePath;
        [Export("filePath")]
        string FilePath { get; }

        // -(void)setObject:(id)value forKey:(NSString *)key;
        [Override]
        [Export("setObject:forKey:")]
        void SetObject(NSObject value, string key);

        // -(id)objectForKey:(NSString *)key;
        [Override]
        [Export("objectForKey:")]
        NSObject GetObject(string key);

        // -(BOOL)synchronize;
        [Override]
        [Export("synchronize")]
        bool Synchronize();
    }

    // @interface IASKSettingsStoreUserDefaults : NSObject <IASKSettingsStore>
    [BaseType(typeof(NSObject), Name = "IASKSettingsStoreUserDefaults")]
    interface SettingsStoreUserDefaults : SettingsStore
    {
        // -(id)initWithUserDefaults:(NSUserDefaults *)defaults;
        [Export("initWithUserDefaults:")]
        IntPtr Constructor(NSUserDefaults defaults);

        // @property (readonly, retain, nonatomic) NSUserDefaults * defaults;
        [Export("defaults", ArgumentSemantic.Retain)]
        NSUserDefaults Defaults { get; }
    }

    // @interface IASKPSSliderSpecifierViewCell : UITableViewCell
    [BaseType (typeof(UITableViewCell), Name = "IASKPSSliderSpecifierViewCell")]
	interface SliderSpecifierViewCell
	{
		// @property (nonatomic, strong) IASKSlider * slider;
		[Export ("slider", ArgumentSemantic.Strong)]
		SettingsSlider Slider { get; set; }

		// @property (nonatomic, strong) UIImageView * minImage;
		[Export ("minImage", ArgumentSemantic.Strong)]
		UIImageView MinImage { get; set; }

		// @property (nonatomic, strong) UIImageView * maxImage;
		[Export ("maxImage", ArgumentSemantic.Strong)]
		UIImageView MaxImage { get; set; }
	}

	// @interface IASKPSTextFieldSpecifierViewCell : UITableViewCell
	[BaseType (typeof(UITableViewCell), Name = "IASKPSTextFieldSpecifierViewCell")]
	interface TextFieldSpecifierViewCell
	{
		// @property (nonatomic, strong) IASKTextField * textField;
		[Export ("textField", ArgumentSemantic.Strong)]
		SettingsTextField TextField { get; set; }
	}

	// @interface IASKSlider : UISlider
	[BaseType (typeof(UISlider), Name = "IASKSlider")]
	interface SettingsSlider
	{
		// @property (copy, nonatomic) NSString * key;
		[Export ("key")]
		string Key { get; set; }
	}

	// @interface IASKSwitch : UISwitch
	[BaseType (typeof(UISwitch), Name = "IASKSwitch")]
	interface SettingsSwitch
	{
		// @property (copy, nonatomic) NSString * key;
		[Export ("key")]
		string Key { get; set; }
	}

	// @interface IASKTextField : UITextField
	[BaseType (typeof(UITextField), Name = "IASKTextField")]
	interface SettingsTextField
	{
		// @property (copy, nonatomic) NSString * key;
		[Export ("key")]
		string Key { get; set; }
	}

    // @interface IASKTextView : UITextView
    [BaseType(typeof(UITextView), Name = "IASKTextView")]
    interface SettingsTextView
    {
        // @property (copy, nonatomic) NSString * key;
        [Export("key")]
        string Key { get; set; }

        // @property (nonatomic, strong) NSString * placeholder;
        [Export("placeholder", ArgumentSemantic.Strong)]
        string Placeholder { get; set; }
    }

    // @interface IASKTextViewCell : UITableViewCell
    [BaseType(typeof(UITableViewCell), Name = "IASKTextViewCell")]
    interface SettingsTextViewCell
    {
        // @property (nonatomic, strong) IASKTextView * textView;
        [Export("textView", ArgumentSemantic.Strong)]
        SettingsTextView TextView { get; set; }
    }
}
