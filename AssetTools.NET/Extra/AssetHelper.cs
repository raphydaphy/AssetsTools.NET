﻿////////////////////////////
//   ASSETSTOOLS.NET PLUGINS
//   Hey, watch out! This   
//   library isn't done yet.
//   You've been warned!    

using System;
using System.Collections.Generic;

namespace AssetsTools.NET.Extra
{
    public static class AssetHelper
    {
        public static ClassDatabaseType FindAssetClassByID(ClassDatabaseFile cldb, uint id)
        {
            uint fixedId = id;
            if (fixedId == 0xf1) //AudioMixerController
                fixedId = 0xf0;  //AudioMixer
            else if (fixedId == 0xf3) //AudioMixerGroupController
                fixedId = 0x111;      //AudioMixerGroup
            else if (fixedId == 0xf5) //AudioMixerSnapshotController
                fixedId = 0x110;      //AudioMixerSnapshot

            foreach (ClassDatabaseType type in cldb.classes)
            {
                if (type.classId == fixedId)
                    return type;
            }
            return null;
        }

        public static ClassDatabaseType FindAssetClassByName(ClassDatabaseFile cldb, string name)
        {
            foreach (ClassDatabaseType type in cldb.classes)
            {
                if (type.name.GetString(cldb) == name)
                    return type;
            }
            return null;
        }

        public static string GetAssetNameFast(AssetsFile file, ClassDatabaseFile cldb, AssetFileInfoEx info)
        {
            ulong pos = info.absoluteFilePos;
            ClassDatabaseType type = FindAssetClassByID(cldb, info.curFileType);

            AssetsFileReader reader = file.reader;

            if (type.fields.Count == 0) return type.name.GetString(cldb);
            if (type.fields[1].fieldName.GetString(cldb) == "m_Name")
            {
                reader.Position = info.absoluteFilePos;
                return reader.ReadCountStringInt32();
            }
            else if (type.name.GetString(cldb) == "GameObject")
            {
                reader.Position = info.absoluteFilePos;
                int size = reader.ReadInt32();
                reader.Position += (ulong)(size * 12);
                reader.Position += 4;
                return reader.ReadCountStringInt32();
            }
            else if (type.name.GetString(cldb) == "MonoBehaviour")
            {
                reader.Position = info.absoluteFilePos;
                reader.Position += 28;
                string name = reader.ReadCountStringInt32();
                if (name != "")
                {
                    return name;
                }
            }
            return type.name.GetString(cldb);
        }

        public static List<AssetFileInfoEx> GetAssetsOfType(this AssetsFileTable table, int typeId)
        {
            List<AssetFileInfoEx> infos = new List<AssetFileInfoEx>();
            foreach (AssetFileInfoEx info in table.pAssetFileInfo)
            {
                if (info.curFileType == typeId)
                {
                    infos.Add(info);
                }
            }
            return infos;
        }
    }
}
