﻿Public Class InspectSpolFragment
    Dim SpolFragment As String
    Public Function PresentDialog(Policy As PolicyPlusPolicy, AdmxWorkspace As AdmxBundle, CompSource As IPolicySource, UserSource As IPolicySource) As DialogResult
        TextPolicyName.Text = Policy.DisplayName
        Dim sb As New Text.StringBuilder
        Dim addSection = Function(Section As AdmxPolicySection) As Boolean
                             If (Policy.RawPolicy.Section And Section) = 0 Then Return False
                             Dim polSource = If(Section = AdmxPolicySection.Machine, CompSource, UserSource)
                             Dim spolState As New SpolPolicyState With {.UniqueID = Policy.UniqueID}
                             spolState.Section = Section
                             spolState.BasicState = PolicyProcessing.GetPolicyState(polSource, Policy)
                             If spolState.BasicState = PolicyState.Enabled Then spolState.ExtraOptions = PolicyProcessing.GetPolicyOptionStates(polSource, Policy)
                             sb.AppendLine(SpolFile.GetFragment(spolState))
                             Return True
                         End Function
        addSection(AdmxPolicySection.Machine)
        addSection(AdmxPolicySection.User)
        SpolFragment = sb.ToString
        UpdateTextbox()
        Return ShowDialog()
    End Function
    Private Sub ButtonCopy_Click(sender As Object, e As EventArgs) Handles ButtonCopy.Click
        TextSpol.SelectAll()
        TextSpol.Copy()
    End Sub
    Private Sub InspectSpolFragment_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        TextSpol.Focus()
        TextSpol.SelectAll()
    End Sub
    Private Sub CheckHeader_CheckedChanged(sender As Object, e As EventArgs) Handles CheckHeader.CheckedChanged
        UpdateTextbox()
    End Sub
    Private Sub UpdateTextbox()
        If CheckHeader.Checked Then
            TextSpol.Text = "Policy Plus Semantic Policy" & vbCrLf & vbCrLf & SpolFragment
        Else
            TextSpol.Text = SpolFragment
        End If
    End Sub
    Private Sub TextSpol_KeyDown(sender As Object, e As KeyEventArgs) Handles TextSpol.KeyDown
        If e.KeyCode = Keys.A And e.Control Then TextSpol.SelectAll()
    End Sub
End Class