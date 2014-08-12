using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using xamarin.circleImageCropperSample.cropper;

namespace Xamarin.CircleImageCropper
{
    [Activity(Label = "Xamarin.CircleImageCropper", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // Static final constants
        private static int DEFAULT_ASPECT_RATIO_VALUES = 10;
        private static int ROTATE_NINETY_DEGREES = 90;
        private static String ASPECT_RATIO_X = "ASPECT_RATIO_X";
        private static String ASPECT_RATIO_Y = "ASPECT_RATIO_Y";
        private static int ON_TOUCH = 1;

        // Instance variables
        private int mAspectRatioX = DEFAULT_ASPECT_RATIO_VALUES;
        private int mAspectRatioY = DEFAULT_ASPECT_RATIO_VALUES;

        Bitmap croppedImage;

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
                this.RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.activity_main);

                // Sets fonts for all
                Typeface mFont = Typeface.CreateFromAsset(Assets, "Roboto-Thin.ttf");
                ViewGroup root = FindViewById<ViewGroup>(Resource.Id.mylayout);
                setFont(root, mFont);

                // Initialize components of the app
                cropImageView cropImageView = FindViewById<cropImageView>(Resource.Id.cropImageView);
                cropImageView.setImageBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.butterfly));
                Spinner showGuidelinesSpin = FindViewById<Spinner>(Resource.Id.showGuidelinesSpin);

                // Set initial spinner value
                showGuidelinesSpin.SetSelection(ON_TOUCH);

                //Set AspectRatio fixed for circular selection
                cropImageView.setFixedAspectRatio(true);

                // Sets initial aspect ratio to 10/10
                cropImageView.setAspectRatio(DEFAULT_ASPECT_RATIO_VALUES, DEFAULT_ASPECT_RATIO_VALUES);

                //Sets the rotate button
                Button rotateButton = FindViewById<Button>(Resource.Id.Button_rotate);
                rotateButton.Click += delegate
                {
                    cropImageView.rotateImage(ROTATE_NINETY_DEGREES);
                };

                // Sets up the Spinner
                //showGuidelinesSpin.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
                //    public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                //        cropImageView.setGuidelines(i);
                //    }

                //    public void onNothingSelected(AdapterView<?> adapterView) {
                //        return;
                //    }
                //});

                Button cropButton = FindViewById<Button>(Resource.Id.Button_crop);
                cropButton.Click += delegate
                {
                    croppedImage = cropImageView.getCroppedCircleImage();
                    ImageView croppedImageView = FindViewById<ImageView>(Resource.Id.croppedImageView);
                    croppedImageView.SetImageBitmap(croppedImage);
                };
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /*
         * Sets the font on all TextViews in the ViewGroup. Searches recursively for
         * all inner ViewGroups as well. Just add a check for any other views you
         * want to set as well (EditText, etc.)
         */
        public void setFont(ViewGroup group, Typeface font)
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
                    setFont((ViewGroup)v, font);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate the menu; this adds items to the action bar if it is present.
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return true;
        }

    }
}

