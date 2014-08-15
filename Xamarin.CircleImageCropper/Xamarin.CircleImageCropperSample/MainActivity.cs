using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.edmodo.cropper;

namespace CircleImageCropper.Sample
{
    [Activity(Label = "CircleImageCropper", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Static final constants
        private static int DEFAULT_ASPECT_RATIO_VALUES = 10;
        private static int ROTATE_NINETY_DEGREES = 90;
        private static String ASPECT_RATIO_X = "ASPECT_RATIO_X";
        private static String ASPECT_RATIO_Y = "ASPECT_RATIO_Y";
        private static int ON_TOUCH = 1;
        private Bitmap croppedImage;
        private CropImageView cropImageView;

        // Instance variables
        private int mAspectRatioX = DEFAULT_ASPECT_RATIO_VALUES;
        private int mAspectRatioY = DEFAULT_ASPECT_RATIO_VALUES;

        // Saves the state upon rotating the screen/restarting the activity
        protected override void OnSaveInstanceState(Bundle bundle)
        {
            base.OnSaveInstanceState(bundle);
            bundle.PutInt(ASPECT_RATIO_X, mAspectRatioX);
            bundle.PutInt(ASPECT_RATIO_Y, mAspectRatioY);
        }

        // Restores the state upon rotating the screen/restarting the activity
        protected override void OnRestoreInstanceState(Bundle bundle)
        {
            base.OnRestoreInstanceState(bundle);
            mAspectRatioX = bundle.GetInt(ASPECT_RATIO_X);
            mAspectRatioY = bundle.GetInt(ASPECT_RATIO_Y);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.activity_main);

                // Sets fonts for all
                Typeface mFont = Typeface.CreateFromAsset(Assets, "Roboto-Thin.ttf");
                var root = FindViewById<ViewGroup>(Resource.Id.mylayout);
                SetFont(root, mFont);

                // Initialize components of the app
                cropImageView = FindViewById<CropImageView>(Resource.Id.CropImageView);
                var showGuidelinesSpin = FindViewById<Spinner>(Resource.Id.showGuidelinesSpin);

                // Set initial spinner value
                showGuidelinesSpin.SetSelection(ON_TOUCH);

                //Set AspectRatio fixed for circular selection
                cropImageView.SetFixedAspectRatio(true);

                // Sets initial aspect ratio to 10/10
                cropImageView.SetAspectRatio(DEFAULT_ASPECT_RATIO_VALUES, DEFAULT_ASPECT_RATIO_VALUES);

                //Sets the rotate button
                var rotateButton = FindViewById<Button>(Resource.Id.Button_rotate);
                rotateButton.Click += delegate { cropImageView.RotateImage(ROTATE_NINETY_DEGREES); };

                // Sets up the Spinner
                showGuidelinesSpin.ItemSelected += showGuidelinesSpin_ItemSelected;
                showGuidelinesSpin.NothingSelected += showGuidelinesSpin_NothingSelected;

                var cropButton = FindViewById<Button>(Resource.Id.Button_crop);
                cropButton.Click += delegate
                {
                    croppedImage = cropImageView.GetCroppedCircleImage();
                    var croppedImageView = FindViewById<ImageView>(Resource.Id.croppedImageView);
                    croppedImageView.SetImageBitmap(croppedImage);
                };
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        void showGuidelinesSpin_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            cropImageView.SetGuidelines(e.Position);
        }

        void showGuidelinesSpin_NothingSelected(object sender, AdapterView.NothingSelectedEventArgs e)
        {
            return;
        }

        /*
         * Sets the font on all TextViews in the ViewGroup. Searches recursively for
         * all inner ViewGroups as well. Just add a check for any other views you
         * want to set as well (EditText, etc.)
         */

        public void SetFont(ViewGroup group, Typeface font)
        {
            try
            {
                int count = group.ChildCount;
                View v;
                for (int i = 0; i < count; i++)
                {
                    v = group.GetChildAt(i);
                    if (v is TextView || v is EditText || v is Button)
                    {
                        ((TextView)v).Typeface = font;
                    }
                    else if (v is ViewGroup)
                    {
                        SetFont((ViewGroup)v, font);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            try
            {
                // Inflate the menu; this adds items to the action bar if it is present.
                MenuInflater.Inflate(Resource.Menu.main, menu);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }
    }
}