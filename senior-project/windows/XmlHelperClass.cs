
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class TestProcedure
{

    private TestProcedureProcedure_Heading procedure_HeadingField;

    private List<TestProcedureSection> sectionsField;

    private TestProcedureRedlinesList redlinesListField;

    /// <remarks/>
    public TestProcedureProcedure_Heading Procedure_Heading
    {
        get
        {
            return this.procedure_HeadingField;
        }
        set
        {
            this.procedure_HeadingField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Section", IsNullable = false)]
    public List<TestProcedureSection> Sections
    {
        get
        {
            return this.sectionsField;
        }
        set
        {
            this.sectionsField = value;
        }
    }

    /// <remarks/>
    public TestProcedureRedlinesList RedlinesList
    {
        get
        {
            return this.redlinesListField;
        }
        set
        {
            this.redlinesListField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureProcedure_Heading
{

    private string nameField;

    private string signatureField;

    private string organizationField;

    private string dateField;

    private string timeField;

    private string load_VersionField;

    private string descriptionField;

    private string systemField;

    private string severityField;

    private string revisionField;

    private string fileName;


    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string Signature
    {
        get
        {
            return this.signatureField;
        }
        set
        {
            this.signatureField = value;
        }
    }

    /// <remarks/>
    public string Organization
    {
        get
        {
            return this.organizationField;
        }
        set
        {
            this.organizationField = value;
        }
    }

    /// <remarks/>
    public string Date
    {
        get
        {
            return this.dateField;
        }
        set
        {
            this.dateField = value;
        }
    }

    /// <remarks/>
    public string Time
    {
        get
        {
            return this.timeField;
        }
        set
        {
            this.timeField = value;
        }
    }

    /// <remarks/>
    public string Load_Version
    {
        get
        {
            return this.load_VersionField;
        }
        set
        {
            this.load_VersionField = value;
        }
    }

    /// <remarks/>
    public string Description
    {
        get
        {
            return this.descriptionField;
        }
        set
        {
            this.descriptionField = value;
        }
    }

    /// <remarks/>
    public string System
    {
        get
        {
            return this.systemField;
        }
        set
        {
            this.systemField = value;
        }
    }

    /// <remarks/>
    public string Severity
    {
        get
        {
            return this.severityField;
        }
        set
        {
            this.severityField = value;
        }
    }

    /// <remarks/>
    public string Revision
    {
        get
        {
            return this.revisionField;
        }
        set
        {
            this.revisionField = value;
        }
    }

    public string FileName
    {
        get
        {
            return this.fileName;
        }
        set
        {
            this.fileName = value;
        }
    }

}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureSection
{

    private string headingField;

    private string descriptionField;

    private List<TestProcedureSectionTest_Step> test_StepField;

    private int idField;

    /// <remarks/>
    public string Heading
    {
        get
        {
            return this.headingField;
        }
        set
        {
            this.headingField = value;
        }
    }

    /// <remarks/>
    public string Description
    {
        get
        {
            return this.descriptionField;
        }
        set
        {
            this.descriptionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Test_Step")]
    public List<TestProcedureSectionTest_Step> Test_Step
    {
        get
        {
            return this.test_StepField;
        }
        set
        {
            this.test_StepField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureSectionTest_Step
{

    private string stationField;

    private string control_ActionField;

    private string expected_ResultField;

    private string passField;

    private string failField;

    private string commentsField;

    private string imageField;

    private int idField;

    /// <remarks/>
    public string Station
    {
        get
        {
            return this.stationField;
        }
        set
        {
            this.stationField = value;
        }
    }

    /// <remarks/>
    public string Control_Action
    {
        get
        {
            return this.control_ActionField;
        }
        set
        {
            this.control_ActionField = value;
        }
    }

    /// <remarks/>
    public string Expected_Result
    {
        get
        {
            return this.expected_ResultField;
        }
        set
        {
            this.expected_ResultField = value;
        }
    }

    /// <remarks/>
    public string Pass
    {
        get
        {
            return this.passField;
        }
        set
        {
            this.passField = value;
        }
    }

    /// <remarks/>
    public string Fail
    {
        get
        {
            return this.failField;
        }
        set
        {
            this.failField = value;
        }
    }

    /// <remarks/>
    public string Comments
    {
        get
        {
            return this.commentsField;
        }
        set
        {
            this.commentsField = value;
        }
    }

    /// <remarks/>
    public string Image
    {
        get
        {
            return this.imageField;
        }
        set
        {
            this.imageField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureRedlinesList
{

    private List<TestProcedureRedlinesListRedlineText> textListField;

    private List<TestProcedureRedlinesListRedlineStep> stepsListField;

    private List<TestProcedureRedlinesListRedlineSection> sectionListField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("RedlineText", IsNullable = false)]
    public List<TestProcedureRedlinesListRedlineText> TextList
    {
        get
        {
            return this.textListField;
        }
        set
        {
            this.textListField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("RedlineStep", IsNullable = false)]
    public List<TestProcedureRedlinesListRedlineStep> StepsList
    {
        get
        {
            return this.stepsListField;
        }
        set
        {
            this.stepsListField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("RedlineSection", IsNullable = false)]
    public List<TestProcedureRedlinesListRedlineSection> SectionList
    {
        get
        {
            return this.sectionListField;
        }
        set
        {
            this.sectionListField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureRedlinesListRedlineText
{

    private string actionToTakeField;

    private string fromSectionField;

    private string fromStepField;

    private string textField;

    private string controlActionField;

    private string expectedResultField;

    private string sectionHeaderField;

    private string sectionDescriptionField;

    private int idField;

    /// <remarks/>
    public string ActionToTake
    {
        get
        {
            return this.actionToTakeField;
        }
        set
        {
            this.actionToTakeField = value;
        }
    }

    /// <remarks/>
    public string SectionDescription
    {
        get
        {
            return this.sectionDescriptionField;
        }
        set
        {
            this.sectionDescriptionField = value;
        }
    }

    /// <remarks/>
    public string SectionHeader
    {
        get
        {
            return this.sectionHeaderField;
        }
        set
        {
            this.sectionHeaderField = value;
        }
    }

    /// <remarks/>
    public string ExpectedResult
    {
        get
        {
            return this.expectedResultField;
        }
        set
        {
            this.expectedResultField = value;
        }
    }

    /// <remarks/>
    public string ControlAction
    {
        get
        {
            return this.controlActionField;
        }
        set
        {
            this.controlActionField = value;
        }
    }

    /// <remarks/>
    public string FromSection
    {
        get
        {
            return this.fromSectionField;
        }
        set
        {
            this.fromSectionField = value;
        }
    }

    /// <remarks/>
    public string FromStep
    {
        get
        {
            return this.fromStepField;
        }
        set
        {
            this.fromStepField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureRedlinesListRedlineStep
{

    private string actionToTakeField;

    private string fromSectionField;

    private string fromStepField;

    private string toSectionField;

    private string toStepField;

    private int idField;

    /// <remarks/>
    public string ActionToTake
    {
        get
        {
            return this.actionToTakeField;
        }
        set
        {
            this.actionToTakeField = value;
        }
    }

    /// <remarks/>
    public string FromSection
    {
        get
        {
            return this.fromSectionField;
        }
        set
        {
            this.fromSectionField = value;
        }
    }

    /// <remarks/>
    public string FromStep
    {
        get
        {
            return this.fromStepField;
        }
        set
        {
            this.fromStepField = value;
        }
    }

    /// <remarks/>
    public string ToSection
    {
        get
        {
            return this.toSectionField;
        }
        set
        {
            this.toSectionField = value;
        }
    }

    /// <remarks/>
    public string ToStep
    {
        get
        {
            return this.toStepField;
        }
        set
        {
            this.toStepField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class TestProcedureRedlinesListRedlineSection
{

    private string actionToTakeField;

    private string fromSectionField;

    private string toSectionField;

    private int idField;

    /// <remarks/>
    public string ActionToTake
    {
        get
        {
            return this.actionToTakeField;
        }
        set
        {
            this.actionToTakeField = value;
        }
    }

    /// <remarks/>
    public string FromSection
    {
        get
        {
            return this.fromSectionField;
        }
        set
        {
            this.fromSectionField = value;
        }
    }

    /// <remarks/>
    public string ToSection
    {
        get
        {
            return this.toSectionField;
        }
        set
        {
            this.toSectionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

}
