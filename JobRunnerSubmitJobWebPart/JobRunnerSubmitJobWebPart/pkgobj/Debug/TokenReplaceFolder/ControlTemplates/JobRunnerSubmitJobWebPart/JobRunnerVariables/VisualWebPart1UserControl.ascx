<%@ Assembly Name="JobRunnerSubmitJobWebPart, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9fe52a956287ef5d" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VisualWebPart1UserControl.ascx.cs"
    Inherits="JobRunnerSubmitJobWebPart.VisualWebPart1.VisualWebPart1UserControl" %>
<script src="../../../_layouts/JScript1.js" type="text/javascript"></script>
<style type="text/css">
    .style1
    {
        width: 234px;
    }
    .style7
    {
        height: 30px;
        width: 557px;
    }
    .style8
    {
        width: 557px;
    }
    .style10
    {
        width: 207px;
    }
</style>
<div style="height: 633px">
    <table style="width: 93%; height: 437px;">
        <tr>
            <td class="style1" title="Load parameters from an XML file created previously">
                Load an XML file (optional)
            </td>
            <td class="style7">
                <asp:FileUpload ID="XMLUploadBrowse" runat="server" />
                <asp:Button ID="XMLUploadButton" Text="Load JobRunner variables" runat="server" OnClick="LoadGuiFromXML"
                    Style="margin-left: 10px" Width="170px" />
            </td>
            <td class="style10">
                <asp:Label ID="xmlFileError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="unique name to append to job files & directory">
                Project description
            </td>
            <td class="style8">
                <asp:TextBox ID="txtPrefix" runat="server" Text="CHS_AA_AF" ReadOnly="false" Width="84px"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="DescriptionError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="How many chromosomes (22/23 for HM; 44/45 for 1000G, 87 for 1000Gv3)">
                Max Number of Chromosomes
            </td>
            <td class="style8">
                <asp:TextBox ID="txtChromosomes" runat="server" Text="23" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td class="style10">
                <asp:Label ID="chromosomeError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="For GEE only: Working Correlation Structure (1=independence, 2=exchangeable,3=AR-M )">
                Correlation Structure
            </td>
            <td class="style8">
                <asp:TextBox ID="txtCorrelation" runat="server" Text="0" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td class="style10">
                <asp:Label ID="correlationError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="adjustment model (for G*E, omit interaction term)">
                Covariates
            </td>
            <td class="style8">
                <asp:TextBox ID="txtCovariates" runat="server" Text="agebl+gend01+factor(clinic)+PC1+PC2+PC3+PC4+PC5+PC6+PC7+PC8+PC9+PC10"
                    ReadOnly="false" Width="531px"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="CovariatesError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="ID variable">
                ID NO
            </td>
            <td class="style8">
                <asp:TextBox ID="txtIdNo" runat="server" Text="idno" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="IdnoError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="variable name for interaction term (if not applicable, type 'none')">
                Interaction term
            </td>
            <td class="style8">
                <asp:TextBox ID="txtInter" runat="server" ReadOnly="false" Text="none" Width="74px"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="InterError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="Regression: 1=linear; 2=logistic; 3=survival | GxE: 4=lin, 5=log, 6=surv |7 = LMM, 8=LMM.int | GEE: 9=lin, 10=lin.int, 11=log; 12=log.int;">
                Model type
            </td>
            <td class="style8">
                <asp:TextBox ID="txtModel" runat="server" Text="3" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td class="style10">
                <asp:Label ID="modelError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="outcome variable name">
                Outcome
            </td>
            <td class="style8">
                <asp:TextBox ID="txtOutcome" runat="server" Text="af_ecghosp" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="OutcomeError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:Label ID="currentDir" runat="server" Visible="false" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td class="style8">
                <asp:ListBox ID="lstDirs" runat="server" Width="170px" Height="200px"></asp:ListBox>
                <asp:ListBox ID="lstFiles" runat="server" Width="275px" Height="200px"></asp:ListBox>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td class="style8">
                <asp:Button ID="cmdBrowsePhenotype" runat="server" Width="170px" Text="Browse"></asp:Button>
                <asp:Button ID="cmdLoadPhenotype" runat="server" Width="275px" Text="Use Phenotype File">
                </asp:Button>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="style1" title="location of phenotype file">
                Phenotype file
            </td>
            <td class="style8">
                <asp:TextBox ID="txtPheno" runat="server" Text="" ReadOnly="false" Width="313px"></asp:TextBox>
                <br />
            </td>
            <td>
                <asp:Label ID="PhenoError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="1=CHS HM2; 2=CHS_AA; 3=HVH1; 4=CHS1KG; 5=HVH2; 1997=CABS_AFA_1kg; 1998=CABS_ASN_1kg; 1999=CHS_EA_1kg ">
                Population type
            </td>
            <td class="style8">
                <asp:TextBox ID="txtPopulation" runat="server" Text="2" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td class="style10">
                <asp:Label ID="populationError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="generate T-based p-values and graphs (1=yes, 0=no)">
                T-based data
            </td>
            <td class="style8">
                <asp:TextBox ID="txtTbased" runat="server" Text="1" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td class="style10">
                <asp:Label ID="tbasedError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1" title="variable name for time-to-event (for multiple observations use 't0,t1') (if empty type 0)">
                Time-to-event
            </td>
            <td class="style8">
                <asp:TextBox ID="txtTtoEvent" runat="server" Text="surv" ReadOnly="false" Width="74px"></asp:TextBox>
            </td>
            <td><asp:Label ID="tToEventError" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            Save XML file (optional)
            </td>
            <td class="style8">
                <asp:Button ID="DownloadXMLButton" Text="Save JobRunner Variables" 
                    Width="206px" runat="server"
                    OnClick="DownloadXML" Style="margin-left: 0px" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td class="style8">
                <asp:Button ID="ExecuteButton" Text="Begin GWAS" Width="256px" runat="server" OnClick="WriteJobFilesAndExecute"
                    Style="margin-left: 0px" BackColor="#00CC00" Height="46px" />
            </td>
            <td><asp:Label ID="ErrorGWAS" runat="server" Text="" ForeColor="Red" Width="255px"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
</div>
