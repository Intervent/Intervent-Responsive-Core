﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.LayoutTransformer
{
    public sealed class BCBSFixedLayoutTransformer : BaseFixedLayoutTransformer
    {
        protected override int[] Positions { get { return new int[] { 15, 24, 54, 89, 99, 116, 126, 156, 181, 216, 226, 229, 230, 238, 288, 338, 368, 370, 380, 400, 408, 416, 417, 421, 431, 437, 487, 491, 493, 543, 593, 623, 625, 635, 639, 640, 641, 663, 671, 679, 687, 695, 703, 711, 714, 722, 725, 734, 743, 752, 760, 768, 776, 784, 786, 791, 793, 797, 799, 802, 811, 820, 822, 833, 844, 855, 866, 877, 888, 899, 910, 921, 932, 935, 937, 940, 943, 946, 949, 952, 955, 958, 961, 964, 967, 969, 980, 1010, 1013, 1038, 1039, 1044, 1046, 1048, 1050, 1053, 1063, 1066, 1072, 1075, 1078, 1088, 1091, 1101, 1111, 1121, 1131, 1132, 1137, 1140, 1143, 1144, 1145, 1147, 1149, 1172, 1176, 1184, 1187, 1190, 1194, 1205, 1208, 1214, 1229, 1244, 1262, 1277, 1280, 1281, 1291, 1341, 1355, 1358 }; } }

        protected override bool IsInsert { get { return true; } }
    }
}
