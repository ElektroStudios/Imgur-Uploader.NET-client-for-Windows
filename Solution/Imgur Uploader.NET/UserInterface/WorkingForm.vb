' ***********************************************************************
' Author   : Elektro
' Modified : 19-January-2015
' ***********************************************************************
' <copyright file="WorkingForm.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Namespace UserInterface

    ''' <summary>
    ''' The Working UserInterface Form.
    ''' </summary>
    Public NotInheritable Class WorkingForm

#Region " Event Handlers "

        ''' <summary>
        ''' Handles the Load event of the Working control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Working_Load(sender As Object, e As EventArgs) _
        Handles MyBase.Load

            Me.CenterForm(Me, Main)

        End Sub

        ''' <summary>
        ''' Handles the Shown event of the Working Form.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Working_Shown(sender As Object, e As EventArgs) _
        Handles MyBase.Shown

            With Me.Timer_StatusLabel
                .Enabled = True
                .Start()
            End With

        End Sub

        ''' <summary>
        ''' Handles the Tick event of the Timer_StatusLabel control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub Timer_StatusLabel_Tick(sender As Object, e As EventArgs) _
        Handles Timer_StatusLabel.Tick

            Select Case Me.Label_Status.Text

                Case "Optimizing image"
                    Me.Label_Status.Text = "Optimizing image."

                Case "Optimizing image."
                    Me.Label_Status.Text = "Optimizing image.."

                Case "Optimizing image.."
                    Me.Label_Status.Text = "Optimizing image..."

                Case "Optimizing image..."
                    Me.Label_Status.Text = "Optimizing image"

            End Select

        End Sub

#End Region

#Region " Methods "

        ''' <summary>
        ''' Centers a form to another form.
        ''' </summary>
        ''' <param name="frm">The FRM.</param>
        ''' <param name="parent">The parent.</param>
        Private Sub CenterForm(ByVal frm As Form, Optional ByVal parent As Form = Nothing)

            Dim r As Rectangle
            If parent IsNot Nothing Then
                r = parent.RectangleToScreen(parent.ClientRectangle)
            Else
                r = Screen.FromPoint(frm.Location).WorkingArea
            End If

            Dim x = r.Left + (r.Width - frm.Width) \ 2
            Dim y = r.Top + (r.Height - frm.Height) \ 2
            frm.Location = New Point(x, y)

        End Sub

#End Region

    End Class

End Namespace