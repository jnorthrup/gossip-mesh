package helloworld;

import com.rokt.gossip.Listener;
import com.rokt.gossip.NodeAddress;
import com.rokt.gossip.NodeState;

import java.util.HashMap;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;

import static com.rokt.gossip.NodeHealth.*;

@SuppressWarnings("unchecked")
class LoadBalancer implements Listener {
    private final Map<Byte, Object> serviceFactories;
    private final Map<Byte, Map<NodeAddress, Object>> services;
    private final Random random;

    public LoadBalancer() {
        this.serviceFactories = new HashMap<>();
        this.services = new HashMap<>();
        this.random = new Random();
    }

    public <T> Service<T> registerService(int serviceByte, ServiceFactory<T> factory) {
        assert !serviceFactories.containsKey((byte) serviceByte);
        serviceFactories.put((byte) serviceByte, factory);
        return new Service<>((byte) serviceByte);
    }

    public class Service<T> {
        private byte serviceByte;

        private Service(byte serviceByte) {
            this.serviceByte = serviceByte;
        }

        public T getEndpoint() {
            Map<NodeAddress, ?> nodes = LoadBalancer.this.services.get(serviceByte);
            if (nodes == null || nodes.isEmpty()) {
                throw new RuntimeException("No services available to handle request");
            } else {
                Object[] arr = nodes.values().toArray(new Object[0]);
                int index = LoadBalancer.this.random.nextInt(arr.length);
                return (T) arr[index];
            }
        }
    }

    private static boolean isAlive(NodeState state) {
        return state != null && (state.health == ALIVE || state.health == SUSPICIOUS);
    }

    private static boolean isDead(NodeState state) {
        return state == null || state.health == DEAD || state.health == LEFT;
    }

    @Override
    public void accept(NodeAddress from, NodeAddress address, NodeState state, NodeState oldState) {
        boolean serviceUpdated = (state != null && oldState != null)
                && (state.serviceByte != oldState.serviceByte)
                && (state.servicePort != oldState.servicePort);
        if (isAlive(oldState) && (isDead(state) || serviceUpdated)) {
            services.computeIfPresent(oldState.serviceByte, (b, nodes) -> {
                Object service = nodes.remove(address);
                if (service != null) {
                    ServiceFactory<Object> factory = (ServiceFactory<Object>) serviceFactories.get(oldState.serviceByte);
                    factory.destroy(service);
                }
                return nodes.isEmpty() ? null : nodes;
            });
        }
        if ((isDead(oldState) && isAlive(state)) || serviceUpdated) {
            ServiceFactory<Object> factory = (ServiceFactory<Object>) serviceFactories.get(state.serviceByte);
            if (factory != null) { // we can't build a client if we don't have the service byte registered!
                services.computeIfAbsent(state.serviceByte, ConcurrentHashMap::new)
                        .put(address, factory.create(address.address, state.servicePort));
            }
        }
    }
}
