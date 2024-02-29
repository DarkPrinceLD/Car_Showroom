﻿Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices

Public Class User_Profile
    Public CustID As Integer
    Private carID As String
    Public loggedIn As Boolean = False
    Private connectionString As String = "Data Source=DESKTOP-R8V9OD0;Initial Catalog=Car_ShowroomA;Integrated Security=True;Encrypt=True; Encrypt=False"
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        drag = True 'Set the flag to indicate dragging is in progress
        mousex = System.Windows.Forms.Cursor.Position.X - Me.Left
        mousey = System.Windows.Forms.Cursor.Position.Y - Me.Top
    End Sub

    Private Sub Form1_Login_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        'Check if dragging is in progress
        If drag Then
            Dim newx As Integer
            Dim newy As Integer
            newx = System.Windows.Forms.Cursor.Position.X - mousex
            newy = System.Windows.Forms.Cursor.Position.Y - mousey
            Me.Location = New Point(newx, newy)
        End If
    End Sub

    Private Sub Form_Login_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        drag = False 'Reset the flag when dragging is complete
    End Sub

    Public Sub CheckOrderConditionsForCustomer(ByVal custID As Integer)
        ' Define your SQL query to fetch orders for the given CustID
        Dim querycheck As String = "SELECT * FROM Orders WHERE CustomerID = @CustID"

        ' Create a SqlConnection
        Using connectioncheck As New SqlConnection(connectionString)
            ' Create a SqlCommand
            Using commandcheck As New SqlCommand(querycheck, connectioncheck)
                ' Add parameter for CustID
                commandcheck.Parameters.AddWithValue("@CustID", custID)

                ' Open the connection
                connectioncheck.Open()

                ' Execute the query and retrieve the result
                Using readercheck As SqlDataReader = commandcheck.ExecuteReader()
                    ' Check if there are any rows returned
                    If readercheck.HasRows Then
                        ' Iterate through the result set
                        While readercheck.Read()
                            ' Retrieve the values of Cart, Ordered, and Delivered columns
                            Dim cart As Boolean = readercheck.GetBoolean(readercheck.GetOrdinal("Cart"))
                            Dim ordered As Boolean = readercheck.GetBoolean(readercheck.GetOrdinal("Ordered"))
                            Dim delivered As Boolean = readercheck.GetBoolean(readercheck.GetOrdinal("Delivered"))
                            carID = readercheck("CarID").ToString()
                            Price2LB.Text = readercheck("Price").ToString()
                            PriceLabel.Text = readercheck("CarID").ToString()
                            CarImage.Image = My.Resources.ResourceManager.GetObject(carID)
                            CarName.Text = GetCarName(carID)
                            ' Check the conditions using if-else statements
                            '********************************************************************************************************************************
                            If cart AndAlso Not ordered AndAlso Not delivered Then
                                ' Condition 1: Cart = 1, Ordered = 0, Delivered = 0
                                ' Perform actions for Condition 1
                                CartElements(True)
                                Console.WriteLine("Condition 1: Cart = 1, Ordered = 0, Delivered = 0")
                                '********************************************************************************************************************************
                            ElseIf Not cart AndAlso ordered AndAlso Not delivered Then
                                ' Condition 2: Cart = 0, Ordered = 1, Delivered = 0
                                ' Perform actions for Condition 2
                                CartElements(False)
                                confoLB.Text = "Your Order is Processing..."
                                Try
                                    Using connection As New SqlConnection(connectionString)
                                        connection.Open()
                                        Dim query As String = "UPDATE Orders SET Cart = 0, Ordered = 0, Delivered = 1 WHERE CustomerID = @custID"
                                        Using command As New SqlCommand(query, connection)
                                            command.Parameters.AddWithValue("@custID", custID)
                                            command.ExecuteNonQuery()
                                        End Using
                                    End Using
                                Catch ex As Exception
                                    MessageBox.Show("Error marking order as successful: " & ex.Message)
                                End Try
                                Console.WriteLine("Condition 2: Cart = 0, Ordered = 1, Delivered = 0")
                                '********************************************************************************************************************************
                            ElseIf Not cart AndAlso Not ordered AndAlso delivered Then
                                ' Condition 3: Cart = 0, Ordered = 0, Delivered = 1
                                ' Perform actions for Condition 3
                                CartElements(False)
                                confoLB.Text = "Your Car is Delivered"
                                Try
                                    Using connection As New SqlConnection(connectionString)
                                        connection.Open()
                                        Dim query As String = "UPDATE Orders SET Cart = 0, Ordered = 0, Delivered = 0, Status = 'ToBeDeleted' WHERE CustomerID = @custID"
                                        Using command As New SqlCommand(query, connection)
                                            command.Parameters.AddWithValue("@custID", custID)
                                            command.ExecuteNonQuery()
                                        End Using
                                    End Using
                                Catch ex As Exception
                                    MessageBox.Show("Error marking order as successful: " & ex.Message)
                                End Try
                                Console.WriteLine("Condition 3: Cart = 0, Ordered = 0, Delivered = 1")
                                '********************************************************************************************************************************
                            ElseIf Not cart AndAlso Not ordered AndAlso Not delivered Then
                                ' Condition 3: Cart = 0, Ordered = 0, Delivered = 0
                                ' Perform actions for Condition 4
                                CartElements(False)
                                confoLB.Text = "Your Car is Delivered"
                                Try
                                    Using connection As New SqlConnection(connectionString)
                                        connection.Open()
                                        Dim query As String = "-- If the status is 'ToBeDeleted', move the tuple to another table OrderHistory
                                                               INSERT INTO OrderHistory (OrderID, CarID, CustomerID, Username, Price, Cart, Ordered, Delivered, Status, Message)
                                                               SELECT OrderID, CarID, CustomerID, Username, Price, Cart, Ordered, Delivered, Status, Message
                                                               FROM Orders WHERE Status = 'ToBeDeleted';

                                                               -- Delete tuples with the status 'ToBeDeleted' from the Order table
                                                               DELETE FROM Orders WHERE Status = 'ToBeDeleted';"
                                        Using command As New SqlCommand(query, connection)
                                            command.Parameters.AddWithValue("@custID", custID)
                                            command.ExecuteNonQuery()
                                        End Using
                                    End Using
                                Catch ex As Exception
                                    MessageBox.Show("Error marking order as successful: " & ex.Message)
                                End Try
                                Console.WriteLine("Condition 4: Cart = 0, Ordered = 0, Delivered = 0")
                                '********************************************************************************************************************************
                            Else
                                ' Other conditions
                                ' Perform actions for other conditions or handle unknown conditions
                                Console.WriteLine("Other conditions")
                            End If
                        End While
                    Else
                        ' No rows returned
                        ' Handle no rows returned
                        confoLB.Text = "No Car In Cart"
                        CartElements(False)
                    End If
                End Using
            End Using
        End Using
    End Sub

    Public Sub CartElements(ByVal Visi As Boolean)
        CarImage.Visible = Visi
        CarName.Visible = Visi
        PriceLabel.Visible = Visi
        ViewMBtn.Visible = Visi
        TotalLB.Visible = Visi
        Price2LB.Visible = Visi
        OrderBtn.Visible = Visi
        trashimg.Visible = Visi
        confoLB.Visible = Not (Visi)

    End Sub

    Private Sub User_Profile_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Sub CustomerInfo(ByVal CustID As String)
        Try
            ' Create a SqlConnection using the connection string
            Using connection As New SqlConnection(connectionString)
                ' Open the connection
                connection.Open()

                ' Define a SQL query to retrieve car details based on CarID
                Dim query As String = "SELECT * FROM customer WHERE CustomerID = @CustID"

                ' Create a SqlCommand with the query and connection
                Using command As New SqlCommand(query, connection)
                    ' Add a parameter for CarID to the SqlCommand
                    command.Parameters.AddWithValue("@CustID", CustID)

                    ' Execute the SQL query and retrieve the results
                    Using reader As SqlDataReader = command.ExecuteReader()
                        ' Check if there are any rows returned
                        If reader.Read() Then
                            ' Populate labels with the retrieved data
                            usernameTxt.Text = reader("Username").ToString()
                            FirstNTxt.Text = reader("FirstName").ToString()
                            LastNTxt.Text = reader("LastName").ToString()
                            EmailTxt.Text = reader("Email").ToString()
                            ContactNoTxt.Text = reader("ContactNumber").ToString()
                            AddressTxt.Text = reader("Address").ToString()
                        Else
                            ' If no rows are returned, display a message
                            MessageBox.Show("No car found with the specified CarID.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Handle any exceptions that may occur
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub
    Public Sub UpdateUI()
        If loggedIn Then
            Registerlink.Visible = False
            Profile.Visible = True
            Logout.Text = "Log out"
            Logout.Visible = True
        Else
            Registerlink.Visible = True
            Logout.Text = ""
            Profile.Visible = False
            Logout.Visible = False
        End If
    End Sub

    Private Sub Homebel_Click(sender As Object, e As EventArgs) Handles Homebel.Click
        Me.Hide()
        Mainform.Show()
        Mainform.loggedIn = loggedIn ' Set loggedIn to True
        Mainform.Profile.Text = "" & Profile.Text
        Mainform.UpdateUI() ' Update the UI in MainForm
    End Sub

    Private Sub Exit_btn_Click(sender As Object, e As EventArgs) Handles Exit_btn.Click
        ' Display a message box with Yes and No buttons
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to exit this application?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        ' Check the user's response
        If result = DialogResult.Yes Then
            ' If the user clicks Yes, close the application
            Application.Exit()
        End If
    End Sub

    Private Sub Logout_Click(sender As Object, e As EventArgs) Handles Logout.Click
        'Display a message box with Yes And No buttons
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to Log out?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        ' Check the user's response
        If result = DialogResult.Yes Then
            ' If the user clicks Yes, close the application
            Me.Hide()
            Form_Login.Show()
            loggedIn = False
            Form_Login.loggedIn = False
            Form_Login.username_txt.Text = ""
            Form_Login.Password_txt.Text = ""
        End If
    End Sub

    Public Function GetCarName(ByVal carId As String) As String
        Dim carName As String = String.Empty

        Try
            Using connection As New SqlConnection(connectionString)
                connection.Open()

                Dim query As String = "SELECT CarName FROM Cars WHERE CarId = @CarId"

                Using command As New SqlCommand(query, connection)
                    command.Parameters.AddWithValue("@CarId", carId)

                    Using reader As SqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            carName = reader("CarName").ToString()
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Handle any exceptions here
            Console.WriteLine("Error: " & ex.Message)
        End Try

        Return carName
    End Function

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub ViewMBtn_Click(sender As Object, e As EventArgs) Handles ViewMBtn.Click
        Me.Hide() ' To hide the login form
        Individual_Car.carID = carID
        Individual_Car.Show() ' To show the main form
        Individual_Car.loggedIn = loggedIn ' Set loggedIn to True
        Individual_Car.Profile.Text = "" & Profile.Text
        Individual_Car.CustID = CustID
        Individual_Car.UpdateUI() ' Update the UI in Individual_Car
        Individual_Car.DisplayCarDetails(carID)
        Individual_Car.ColorsDisplay(carID)
    End Sub

    Private Sub trashimg_Click(sender As Object, e As EventArgs) Handles trashimg.Click
        ' Display a message box with Yes and No buttons
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to Delete this Car from Cart?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        ' Check the user's response
        If result = DialogResult.Yes Then
            Try
                Using connection As New SqlConnection(connectionString)
                    connection.Open()
                    Dim query As String = "DELETE FROM Orders WHERE CustomerID = @custID;"
                    Using command As New SqlCommand(query, connection)
                        command.Parameters.AddWithValue("@custID", CustID)
                        command.ExecuteNonQuery()
                    End Using
                End Using
                Mainform.Show()
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("Error marking order as successful: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub OrderBtn_Click(sender As Object, e As EventArgs) Handles OrderBtn.Click
        ' Display a message box with Yes and No buttons
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to Order this Car?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
        ' Check the user's response
        If result = DialogResult.Yes Then
            Payment.ShowDialog()
            Payment.CustID = CustID
        End If
    End Sub
End Class

