kubectl apply -f https://raw.githubusercontent.com/nats-io/nats-operator/master/deploy/default-rbac.yaml

kubectl apply -f https://raw.githubusercontent.com/nats-io/nats-operator/master/deploy/deployment.yaml

kubectl apply -f ./nats-cluster.yaml