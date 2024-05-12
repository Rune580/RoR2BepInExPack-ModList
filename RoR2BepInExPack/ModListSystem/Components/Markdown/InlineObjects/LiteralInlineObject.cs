using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig.Syntax.Inlines;
using RoR2.UI;
using RoR2BepInExPack.ModListSystem.Markdown;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LiteralInlineObject : BaseMarkdownInlineObject
{
    // This fucking monstrosity is required for detecting all emojis from v15.1 https://unicode.org/Public/emoji/15.1/emoji-test.txt
    // Generated using https://github.com/Felk/UnicodeEmojiRegex with updated emoji-test.txt and emoji-data.txt
    //
    // For anyone coming here looking to port this to their unity project, if you have TMP v3.2+ you can just use a font with the icons, but for those of us that can't, this is what we're stuck with.
    // If you're fine not having colored icons, you can still use the TMP method.
    private static readonly Regex UnicodeRegex = new(
        @"((\uD83D(?:\uDC68(?:\uD83C(?:\uDFFB(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFC-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFC(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB\uDFFD-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFD(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFE(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFD\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFF(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFE]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?)|\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D)?\uDC68|\uD83D(?:(?:[\uDC68\uDC69]\u200D\uD83D)?(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|\uD83E(?:[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]))?|\uDC69(?:\uD83C(?:\uDFFB(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D[\uDC68\uDC69]\uD83C[\uDFFC-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFC(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D[\uDC68\uDC69]\uD83C[\uDFFB\uDFFD-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFD(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D[\uDC68\uDC69]\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFE(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D[\uDC68\uDC69]\uD83C[\uDFFB-\uDFFD\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFF(?:\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])\uD83C[\uDFFB-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83D[\uDC68\uDC69]\uD83C[\uDFFB-\uDFFE]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?)|\u200D(?:\u2764\uFE0F?\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])|\uD83D(?:(?:\uDC69\u200D\uD83D)?(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92])|\uD83E(?:[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF93\uDFA4\uDFA8\uDFEB\uDFED]))?|\uDEB6(?:\uD83C[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F(?:\u200D\u27A1\uFE0F?)?|(?:[\u2640\u2642]\u200D)?\u27A1\uFE0F?|[\u2640\u2642]))?|\u200D(?:[\u2640\u2642]\uFE0F(?:\u200D\u27A1\uFE0F?)?|(?:[\u2640\u2642]\u200D)?\u27A1\uFE0F?|[\u2640\u2642]))?|(?:\uDD75(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F)?|\uDC6F)(?:\u200D[\u2640\u2642]\uFE0F?)?|[\uDC6E\uDC70\uDC71\uDC73\uDC77\uDC81\uDC82\uDC86\uDC87\uDE45-\uDE47\uDE4B\uDE4D\uDE4E\uDEA3\uDEB4\uDEB5](?:\uD83C[\uDFFB-\uDFFF](?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|\uDC41(?:\uFE0F(?:\u200D\uD83D\uDDE8\uFE0F?)?|\u200D\uD83D\uDDE8\uFE0F?)?|\uDE36(?:\u200D\uD83C\uDF2B\uFE0F?)?|\uDC15(?:\u200D\uD83E\uDDBA)?|\uDC26(?:\u200D(?:\uD83D\uDD25|\u2B1B))?|\uDC3B(?:\u200D\u2744\uFE0F?)?|\uDE2E(?:\u200D\uD83D\uDCA8)?|\uDE35(?:\u200D\uD83D\uDCAB)?|\uDE42(?:\u200D[\u2194\u2195]\uFE0F?)?|[\uDC42\uDC43\uDC46-\uDC50\uDC66\uDC67\uDC6B-\uDC6D\uDC72\uDC74-\uDC76\uDC78\uDC7C\uDC83\uDC85\uDC8F\uDC91\uDCAA\uDD7A\uDD95\uDD96\uDE4C\uDE4F\uDEC0\uDECC](?:\uD83C[\uDFFB-\uDFFF])?|[\uDD74\uDD90]\uD83C[\uDFFB-\uDFFF]|\uDC08(?:\u200D\u2B1B)?|[\uDC3F\uDCFD\uDD49\uDD4A\uDD6F\uDD70\uDD73\uDD74\uDD76-\uDD79\uDD87\uDD8A-\uDD8D\uDD90\uDDA5\uDDA8\uDDB1\uDDB2\uDDBC\uDDC2-\uDDC4\uDDD1-\uDDD3\uDDDC-\uDDDE\uDDE1\uDDE3\uDDE8\uDDEF\uDDF3\uDDFA\uDECB\uDECD-\uDECF\uDEE0-\uDEE5\uDEE9\uDEF0\uDEF3]\uFE0F?|[\uDC00-\uDC07\uDC09-\uDC14\uDC16-\uDC25\uDC27-\uDC3A\uDC3C-\uDC3E\uDC40\uDC44\uDC45\uDC51-\uDC65\uDC6A\uDC79-\uDC7B\uDC7D-\uDC80\uDC84\uDC88-\uDC8E\uDC90\uDC92-\uDCA9\uDCAB-\uDCFC\uDCFF-\uDD3D\uDD4B-\uDD4E\uDD50-\uDD67\uDDA4\uDDFB-\uDE2D\uDE2F-\uDE34\uDE37-\uDE41\uDE43\uDE44\uDE48-\uDE4A\uDE80-\uDEA2\uDEA4-\uDEB3\uDEB7-\uDEBF\uDEC1-\uDEC5\uDED0-\uDED2\uDED5-\uDED7\uDEDC-\uDEDF\uDEEB\uDEEC\uDEF4-\uDEFC\uDFE0-\uDFEB\uDFF0])|\uD83E(?:\uDDD1(?:\uD83C(?:\uDFFB(?:\u200D(?:\u2764\uFE0F?\u200D(?:\uD83D\uDC8B\u200D)?\uD83E\uDDD1\uD83C[\uDFFC-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFC(?:\u200D(?:\u2764\uFE0F?\u200D(?:\uD83D\uDC8B\u200D)?\uD83E\uDDD1\uD83C[\uDFFB\uDFFD-\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFD(?:\u200D(?:\u2764\uFE0F?\u200D(?:\uD83D\uDC8B\u200D)?\uD83E\uDDD1\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFE(?:\u200D(?:\u2764\uFE0F?\u200D(?:\uD83D\uDC8B\u200D)?\uD83E\uDDD1\uD83C[\uDFFB-\uDFFD\uDFFF]|\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDFFF(?:\u200D(?:\u2764\uFE0F?\u200D(?:\uD83D\uDC8B\u200D)?\uD83E\uDDD1\uD83C[\uDFFB-\uDFFE]|\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?)|\u200D(?:\uD83E(?:(?:\uDDD1\u200D\uD83E)?\uDDD2(?:\u200D\uD83E\uDDD2)?|[\uDDAF\uDDBC\uDDBD](?:\u200D\u27A1\uFE0F?)?|\uDD1D\u200D\uD83E\uDDD1|[\uDDB0-\uDDB3])|[\u2695\u2696\u2708]\uFE0F?|\uD83C[\uDF3E\uDF73\uDF7C\uDF84\uDF93\uDFA4\uDFA8\uDFEB\uDFED]|\uD83D[\uDCBB\uDCBC\uDD27\uDD2C\uDE80\uDE92]))?|\uDDCE(?:\uD83C[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F(?:\u200D\u27A1\uFE0F?)?|(?:[\u2640\u2642]\u200D)?\u27A1\uFE0F?|[\u2640\u2642]))?|\u200D(?:[\u2640\u2642]\uFE0F(?:\u200D\u27A1\uFE0F?)?|(?:[\u2640\u2642]\u200D)?\u27A1\uFE0F?|[\u2640\u2642]))?|\uDEF1(?:\uD83C(?:\uDFFB(?:\u200D\uD83E\uDEF2\uD83C[\uDFFC-\uDFFF])?|\uDFFC(?:\u200D\uD83E\uDEF2\uD83C[\uDFFB\uDFFD-\uDFFF])?|\uDFFD(?:\u200D\uD83E\uDEF2\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF])?|\uDFFE(?:\u200D\uD83E\uDEF2\uD83C[\uDFFB-\uDFFD\uDFFF])?|\uDFFF(?:\u200D\uD83E\uDEF2\uD83C[\uDFFB-\uDFFE])?))?|[\uDD26\uDD35\uDD37-\uDD39\uDD3D\uDD3E\uDDB8\uDDB9\uDDCD\uDDCF\uDDD4\uDDD6-\uDDDD](?:\uD83C[\uDFFB-\uDFFF](?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|[\uDD3C\uDDDE\uDDDF](?:\u200D[\u2640\u2642]\uFE0F?)?|[\uDD0C\uDD0F\uDD18-\uDD1F\uDD30-\uDD34\uDD36\uDD77\uDDB5\uDDB6\uDDBB\uDDD2\uDDD3\uDDD5\uDEC3-\uDEC5\uDEF0\uDEF2-\uDEF8](?:\uD83C[\uDFFB-\uDFFF])?|[\uDD0D\uDD0E\uDD10-\uDD17\uDD20-\uDD25\uDD27-\uDD2F\uDD3A\uDD3F-\uDD45\uDD47-\uDD76\uDD78-\uDDB4\uDDB7\uDDBA\uDDBC-\uDDCC\uDDD0\uDDE0-\uDDFF\uDE70-\uDE7C\uDE80-\uDE88\uDE90-\uDEBD\uDEBF-\uDEC2\uDECE-\uDEDB\uDEE0-\uDEE8])|\uD83C(?:\uDFF4(?:\uDB40\uDC67\uDB40\uDC62\uDB40(?:\uDC65\uDB40\uDC6E\uDB40\uDC67|\uDC73\uDB40\uDC63\uDB40\uDC74|\uDC77\uDB40\uDC6C\uDB40\uDC73)\uDB40\uDC7F|\u200D\u2620\uFE0F?)?|\uDFC3(?:\uD83C[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F(?:\u200D\u27A1\uFE0F?)?|(?:[\u2640\u2642]\u200D)?\u27A1\uFE0F?|[\u2640\u2642]))?|\u200D(?:[\u2640\u2642]\uFE0F(?:\u200D\u27A1\uFE0F?)?|(?:[\u2640\u2642]\u200D)?\u27A1\uFE0F?|[\u2640\u2642]))?|[\uDFC4\uDFCA](?:\uD83C[\uDFFB-\uDFFF](?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|[\uDFCB\uDFCC](?:\uD83C[\uDFFB-\uDFFF]|\uFE0F)(?:\u200D[\u2640\u2642]\uFE0F?)?|\uDFF3(?:\uFE0F(?:\u200D(?:\u26A7\uFE0F?|\uD83C\uDF08))?|\u200D(?:\u26A7\uFE0F?|\uD83C\uDF08))?|(?:[\uDFCB\uDFCC]\u200D[\u2640\u2642]|[\uDD70\uDD71\uDD7E\uDD7F\uDE02\uDE37\uDF21\uDF24-\uDF2C\uDF36\uDF7D\uDF96\uDF97\uDF99-\uDF9B\uDF9E\uDF9F\uDFCD\uDFCE\uDFD4-\uDFDF\uDFF5\uDFF7])\uFE0F?|\uDF44(?:\u200D\uD83D\uDFEB)?|\uDF4B(?:\u200D\uD83D\uDFE9)?|[\uDF85\uDFC2\uDFC7](?:\uD83C[\uDFFB-\uDFFF])?|\uDDE6\uD83C[\uDDE8-\uDDEC\uDDEE\uDDF1\uDDF2\uDDF4\uDDF6-\uDDFA\uDDFC\uDDFD\uDDFF]|\uDDE7\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEF\uDDF1-\uDDF4\uDDF6-\uDDF9\uDDFB\uDDFC\uDDFE\uDDFF]|\uDDE8\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDEE\uDDF0-\uDDF5\uDDF7\uDDFA-\uDDFF]|\uDDE9\uD83C[\uDDEA\uDDEC\uDDEF\uDDF0\uDDF2\uDDF4\uDDFF]|\uDDEA\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDED\uDDF7-\uDDFA]|\uDDEB\uD83C[\uDDEE-\uDDF0\uDDF2\uDDF4\uDDF7]|\uDDEC\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEE\uDDF1-\uDDF3\uDDF5-\uDDFA\uDDFC\uDDFE]|\uDDED\uD83C[\uDDF0\uDDF2\uDDF3\uDDF7\uDDF9\uDDFA]|\uDDEE\uD83C[\uDDE8-\uDDEA\uDDF1-\uDDF4\uDDF6-\uDDF9]|\uDDEF\uD83C[\uDDEA\uDDF2\uDDF4\uDDF5]|\uDDF0\uD83C[\uDDEA\uDDEC-\uDDEE\uDDF2\uDDF3\uDDF5\uDDF7\uDDFC\uDDFE\uDDFF]|\uDDF1\uD83C[\uDDE6-\uDDE8\uDDEE\uDDF0\uDDF7-\uDDFB\uDDFE]|\uDDF2\uD83C[\uDDE6\uDDE8-\uDDED\uDDF0-\uDDFF]|\uDDF3\uD83C[\uDDE6\uDDE8\uDDEA-\uDDEC\uDDEE\uDDF1\uDDF4\uDDF5\uDDF7\uDDFA\uDDFF]|\uDDF4\uD83C\uDDF2|\uDDF5\uD83C[\uDDE6\uDDEA-\uDDED\uDDF0-\uDDF3\uDDF7-\uDDF9\uDDFC\uDDFE]|\uDDF6\uD83C\uDDE6|\uDDF7\uD83C[\uDDEA\uDDF4\uDDF8\uDDFA\uDDFC]|\uDDF8\uD83C[\uDDE6-\uDDEA\uDDEC-\uDDF4\uDDF7-\uDDF9\uDDFB\uDDFD-\uDDFF]|\uDDF9\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDED\uDDEF-\uDDF4\uDDF7\uDDF9\uDDFB\uDDFC\uDDFF]|\uDDFA\uD83C[\uDDE6\uDDEC\uDDF2\uDDF3\uDDF8\uDDFE\uDDFF]|\uDDFB\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDEE\uDDF3\uDDFA]|\uDDFC\uD83C[\uDDEB\uDDF8]|\uDDFD\uD83C\uDDF0|\uDDFE\uD83C[\uDDEA\uDDF9]|\uDDFF\uD83C[\uDDE6\uDDF2\uDDFC]|[\uDC04\uDCCF\uDD8E\uDD91-\uDD9A\uDE01\uDE1A\uDE2F\uDE32-\uDE36\uDE38-\uDE3A\uDE50\uDE51\uDF00-\uDF20\uDF2D-\uDF35\uDF37-\uDF43\uDF45-\uDF4A\uDF4C-\uDF7C\uDF7E-\uDF84\uDF86-\uDF93\uDFA0-\uDFC1\uDFC5\uDFC6\uDFC8\uDFC9\uDFCB\uDFCC\uDFCF-\uDFD3\uDFE0-\uDFF0\uDFF8-\uDFFF])|\u26F9(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F)(?:\u200D[\u2640\u2642]\uFE0F?)?|\u200D[\u2640\u2642]\uFE0F?)?|\u26D3(?:\uFE0F(?:\u200D\uD83D\uDCA5)?|\u200D\uD83D\uDCA5)?|\u2764(?:\uFE0F(?:\u200D(?:\uD83D\uDD25|\uD83E\uDE79))?|\u200D(?:\uD83D\uDD25|\uD83E\uDE79))?|[\#\*0-9]\uFE0F?\u20E3|[\u261D\u270C\u270D]\uD83C[\uDFFB-\uDFFF]|[\u270A\u270B](?:\uD83C[\uDFFB-\uDFFF])?|[\u00A9\u00AE\u203C\u2049\u2122\u2139\u2194-\u2199\u21A9\u21AA\u2328\u23CF\u23ED-\u23EF\u23F1\u23F2\u23F8-\u23FA\u24C2\u25AA\u25AB\u25B6\u25C0\u25FB\u25FC\u2600-\u2604\u260E\u2611\u2618\u261D\u2620\u2622\u2623\u2626\u262A\u262E\u262F\u2638-\u263A\u2640\u2642\u265F\u2660\u2663\u2665\u2666\u2668\u267B\u267E\u2692\u2694-\u2697\u2699\u269B\u269C\u26A0\u26A7\u26B0\u26B1\u26C8\u26CF\u26D1\u26E9\u26F0\u26F1\u26F4\u26F7\u26F8\u2702\u2708\u2709\u270C\u270D\u270F\u2712\u2714\u2716\u271D\u2721\u2733\u2734\u2744\u2747\u2763\u27A1\u2934\u2935\u2B05-\u2B07\u3030\u303D\u3297\u3299]\uFE0F?|[\u231A\u231B\u23E9-\u23EC\u23F0\u23F3\u25FD\u25FE\u2614\u2615\u2648-\u2653\u267F\u2693\u26A1\u26AA\u26AB\u26BD\u26BE\u26C4\u26C5\u26CE\u26D4\u26EA\u26F2\u26F3\u26F5\u26FA\u26FD\u2705\u2728\u274C\u274E\u2753-\u2755\u2757\u2795-\u2797\u27B0\u27BF\u2B1B\u2B1C\u2B50\u2B55])+)",
        RegexOptions.Compiled
    );
    
    public HGTextMeshProUGUI label;
    public LayoutElement layoutElement;
    public GameObject emojiPrefab;
    
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!label || !layoutElement || !emojiPrefab)
            return;

        if (inline is not LiteralInline literalInline)
            return;

        label.fontSize = inlineCtx.FontSize;
        
        var text = literalInline.ToString();

        SetText(text, renderCtx, inlineCtx);
    }

    protected void SetText(string text, RenderContext renderCtx, InlineContext inlineCtx)
    {
        var emojis = ParseEmojis(text);
        
        AnchoredYPos = -inlineCtx.YPos;

        var fontStyle = FontStyles.Normal;

        var styling = inlineCtx.Styling;

        if (styling.HasTag("b"))
            fontStyle |= FontStyles.Bold;
        if (styling.HasTag("i"))
            fontStyle |= FontStyles.Italic;

        var wrapPerChar = this is CodeInlineObject;
        var lineWidths = TextWidthMultiLine(text, fontStyle, emojis, inlineCtx, renderCtx.ViewportRect.width, wrapPerChar, out var textPerLine, out var positionedEmojis);
        
        CreateEmojis(positionedEmojis, inlineCtx);
        
        var textWithoutLastLine = string.Join("\n", textPerLine.AsEnumerable().Take(textPerLine.Length - 1));
        var height = TextHeight(textWithoutLastLine);
        
        for (var i = 0; i < textPerLine.Length; i++)
        {
            if (string.IsNullOrEmpty(textPerLine[i]))
                textPerLine[i] = " ";
            
            if (i == 0)
            {
                textPerLine[i] = $"<line-indent={inlineCtx.XPos}px>{styling.StyledTemplate.Replace("{0}", textPerLine[i])}</line-indent>";
                continue;
            }
            
            textPerLine[i] = $"{styling.StyledTemplate.Replace("{0}", textPerLine[i])}";
        }
        
        label.SetText(string.Join("\n", textPerLine));

        foreach (var lineWidth in lineWidths)
            inlineCtx.SetPreferredWidthIfBigger(lineWidth);

        inlineCtx.XPos = lineWidths.Last();

        if (lineWidths.Length > 1 && !inlineCtx.LastItem)
        {
            inlineCtx.YPos += height;
        }
        else if (inlineCtx.LastItem)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
            inlineCtx.YPos += label.preferredHeight;
        }
    }

    private void CreateEmojis(EmojiPosition[] positionedEmojis, InlineContext inlineCtx)
    {
        var size = new Vector2(inlineCtx.FontSize, inlineCtx.FontSize);
        
        foreach (var emoji in positionedEmojis)
        {
            var instance = Instantiate(emojiPrefab, transform);
            var controller = instance.GetComponent<EmojiController>();

            controller.rt.anchoredPosition = emoji.Position;
            controller.rt.sizeDelta = size;
            
            controller.SetEmoji(emoji.CodePoint, inlineCtx.FontSize);

            layoutElement.minHeight = emoji.Position.y + size.y;
        }
    }

    private float[] TextWidthMultiLine(string text, FontStyles style, EmojiData[] emojis, InlineContext inlineCtx, float maxWidth, bool wrapPerChar, out string[] textPerLine, out EmojiPosition[] positionedEmojis)
    {
        var heightPerLine = TextHeight(" ");
        
        int charIndex = 0;
        
        int line = 0;
        List<float> lineWidths = [inlineCtx.XPos];
        List<string> textPerLineList = [""];
        
        int emojiIndex = 0;
        int charsToSkip = 0;
        List<EmojiPosition> emojiPositions = [];

        var word = "";
        var wordWidth = 0f;
        foreach (var character in text)
        {
            var charWidth = CharacterWidthApproximation(character, style);

            var lineWidth = lineWidths[line];
            var lineText = textPerLineList[line];
            
            if (emojiIndex < emojis.Length)
            {
                var emoji = emojis[emojiIndex];
            
                if (emoji.CharIndex == charIndex)
                {
                    emojiIndex++;
                    charsToSkip = emoji.Length;
                    
                    emojiPositions.Add(new EmojiPosition(emoji.CodePoint, new Vector2(lineWidth, line * heightPerLine)));
                    
                    lineText += $"<space={inlineCtx.FontSize}px>";
                    lineWidth += inlineCtx.FontSize;
                    
                    lineWidths[line] = lineWidth;
                    textPerLineList[line] = lineText;
                }
            }
            
            charIndex++;

            if (charWidth == 0) // Glyph isn't valid, skip 
                continue;

            if (charsToSkip > 0)
            {
                charsToSkip--;
                continue;
            }

            if (lineWidth + wordWidth + charWidth >= maxWidth)
            {
                lineWidth = 0;
                lineWidths.Add(lineWidth);
                
                line++;
                
                textPerLineList.Add("");
                lineText = textPerLineList[line];
            }

            if (wrapPerChar)
            {
                lineText += character;
                lineWidth += charWidth;
            }
            else
            {
                word += character;
                wordWidth += charWidth;
                
                if (character is ' ' or '\n')
                {
                    lineText += word;
                    lineWidth += wordWidth;
                
                    word = "";
                    wordWidth = 0;
                }
            }

            lineWidths[line] = lineWidth;
            textPerLineList[line] = lineText;
        }

        lineWidths[line] += wordWidth;
        textPerLineList[line] += word;

        textPerLine = textPerLineList.ToArray();
        positionedEmojis = emojiPositions.ToArray();

        return lineWidths.ToArray();
    }
    
    private float TextWidthApproximation(string text, FontStyles style)
    {
        if (!label)
            return 0;
        
        var fontSize = label.fontSize;
        TMP_FontAsset fontAsset = label.font;

        // Compute scale of the target point size relative to the sampling point size of the font asset.
        float pointSizeScale = fontSize / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
        float emScale = fontSize * 0.01f;

        float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
        float normalSpacingAdjustment = fontAsset.normalSpacingOffset;

        float width = 0;

        foreach (var unicode in text)
        {
            // Make sure the given unicode exists in the font asset.
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out var character))
                width += character.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
        }

        return width;
    }

    private float CharacterWidthApproximation(char character, FontStyles style)
    {
        if (!label)
            return 0;

        var fontSize = label.fontSize;
        var fontAsset = label.font;
        
        // Compute scale of the target point size relative to the sampling point size of the font asset.
        float pointSizeScale = fontSize / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
        float emScale = fontSize * 0.01f;
        
        float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
        float normalSpacingAdjustment = fontAsset.normalSpacingOffset;

        if (!fontAsset.characterLookupTable.TryGetValue(character, out var fontChar))
            return 0;
        
        return fontChar.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
    }

    // Yes I know this is very lazy.
    protected float TextHeight(string text)
    {
        if (!label)
            return 0;

        if (string.IsNullOrEmpty(text))
        {
            label.SetText(" ");
        }
        else
        {
            label.SetText(text);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(label.rectTransform);
        
        return label.preferredHeight;
    }
    
    private static EmojiData[] ParseEmojis(string text)
    {
        var matches = UnicodeRegex.Matches(text);
        if (matches.Count <= 0)
            return [];

        var emojis = new List<EmojiData>();
        foreach (Match match in matches)
        {
            var captures = match.Groups[2].Captures;

            foreach (Capture capture in captures)
            {
                string codePoint = "";
                var emoji = capture.Value;

                var i = 0;
                while (i < emoji.Length)
                {
                    if (i > 0)
                        codePoint += "-";

                    var hexCode = Char.ConvertToUtf32(emoji, i);

                    codePoint += hexCode.ToString("x");

                    i += Char.IsSurrogatePair(emoji, i) ? 2 : 1;
                }

                emojis.Add(new EmojiData(codePoint, capture.Index, capture.Length));
            }
        }

        return emojis.ToArray();
    }
    
    private readonly struct EmojiData(string codePoint, int charIndex, int length)
    {
        public readonly string CodePoint = codePoint;
        public readonly int CharIndex = charIndex;
        public readonly int Length = length;
    }
    
    private readonly struct EmojiPosition(string codePoint, Vector2 position)
    {
        public readonly string CodePoint = codePoint;
        public readonly Vector2 Position = position;
    }
}
