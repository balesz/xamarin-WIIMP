using System;
using UIKit;

namespace ios
{
    public static class Material
    {
        public static nfloat ActionBarHeight { get {
                return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 64f : 56f;
            }
        }

        public static nfloat DrawerMaskMaxAlpha { get {
                return 0.75f;
            }
        }
    }
}

