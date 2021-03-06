/**
 * C# Arrange of GNUGO 1.2
 * 
 * modified by Muzudho
 * last modified 2015-11-28
 *  
 * findsavr.c -> Util_SasiteSaver.cs
 */
/*
                 GNUGO - the game of Go (Wei-Chi)
                Version 1.2   last revised 10-31-95
           Copyright (C) Free Software Foundation, Inc.
                      written by Man L. Li
                      modified by Wayne Iba
        modified by Frank Pursel <fpp%minor.UUCP@dragon.com>
                    documented by Bob Webber
*/
/*
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation - version 2.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License in file COPYING for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

Please report any bug/fix, modification, suggestion to

           manli@cs.uh.edu
*/
using Grayscale.GPL.P___160_Collection_.L500_Collection;
using Grayscale.GPL.P___190_Board______.L250_Board;
using Grayscale.GPL.P___300_Taikyoku___.L500_Taikyoku;
using Grayscale.GPL.P160____Collection_.L500_Collection;
using Grayscale.GPL.P403____CompThink__.L125_SasiteNext;

namespace Grayscale.GPL.P403____CompThink__.L250_SasiteSaver
{
    /// <summary>
    /// 石たちの攻撃からの防御のためのコンピューターの次の動きを探します。
    /// 
    /// コンピューターの思考中に使われます。
    /// </summary>
    public abstract class Util_SasiteSaver
    {
        /// <summary>
        /// もし、幾つかのピースズが脅かされているなら、動きを探します。
        /// 
        /// Gnugo1.2 では findsaver 関数。
        /// </summary>
        /// <param name="out_bestLocation">次の指し手の   行、列番号</param>
        /// <param name="out_maxScore">Gnugo1.2 では、val 引数。次の指し手の評価値</param>
        /// <param name="libertyOfNodes">
        /// カレント色の（１つ、または連の）石のリバティー（四方の石を置けるところ）
        /// 
        /// Gnugo1.2 では、l という名前のグローバル変数。liberty の略だろうか？
        /// eval で内容が設定され、その内容は exambord、findsavr、findwinr、suicideで使用されます。
        /// </param>
        /// <param name="taikyoku"></param>
        /// <returns></returns>
        public static bool FindLocation_LibertyWeak
        (
            out GobanPoint out_bestLocation,
            out int out_maxScore,
            int[,] libertyOfNodes,
            Taikyoku taikyoku
        )
        {
            Board ban = taikyoku.Goban; // 碁盤
            int banSize = taikyoku.GobanBounds.BoardSize; //何路盤

            out_bestLocation = new GobanPointImpl(-1,-1);// 位置i,j
            out_maxScore = -1;  // 評価値

            //
            // まず　リバティが１つ、
            // 次に　リバティが２つ、
            // と、リバティの少ない方から順に評価を付けていきます。
            // 盤面を４回探し回ることになります。
            //
            for (int iExpectedLiberty = 1; iExpectedLiberty < 4; iExpectedLiberty++)// リバティー 1〜3 のループカウンター。Gnugo1.2 では minlib 変数。
            {
                // 最小リバティーといっしょのピースを数えます。
                for (int m = 0; m < banSize; m++)
                {
                    for (int n = 0; n < banSize; n++)
                    {
                        GobanPoint iLocation = new GobanPointImpl(m, n);//検索中の位置。

                        if
                        (
                            ban.At(iLocation) == taikyoku.MyColor // 脅かされているかもしれない自分（コンピューター）の色
                            &&
                            libertyOfNodes[m,n] == iExpectedLiberty // 呼吸点の数が少ないかもしれない
                        )
                        {
                            GobanPoint bestLocation; // Gnugo1.2 では、ti,tj (try i,j)という変数名。
                            int bestScore;    // Gnugo1.2 では、tval 変数。評価値

                            // セーブ・ピースズへの動きを探します。
                            taikyoku.MarkingBoard.Initmark(taikyoku.GobanBounds.BoardSize);
                            if
                            (
                                Util_SasiteNext.FindStone_LibertyWeak(out bestLocation, out bestScore, iLocation, iExpectedLiberty, taikyoku)
                                &&
                                (out_maxScore < bestScore)   // 評価値が高い（リバティーが少なくて危険）ならば
                            )
                            {
                                out_maxScore = bestScore;    // 評価値
                                out_bestLocation.SetLocation(bestLocation);// 位置i,j
                            }
                        }
                    }
                }
            }

            if (0 < out_maxScore)   // 動きが見つかれば
            {
                return true;
            }
            else    // 動きが見つからなかったら
            {
                return false;
            }
        }
    }
}