SELECT Customer.CustomerId, RegistrationDateTime FROM Customer 
INNER JOIN Purchaise ON Customer.CustomerId = Purchaise.CustomerId 
WHERE ProductName = '������' AND 
(SELECT COUNT(aPurchaise.ProductName) FROM Purchaise AS aPurchaise
WHERE aPurchaise.CustomerId=Customer.CustomerId AND aPurchaise.ProductName = '�������')=0
GROUP BY Customer.CustomerId, RegistrationDateTime