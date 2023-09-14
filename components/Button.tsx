import { TouchableOpacity, Text } from "react-native";

type ButtonProps = {
  label: string;
  onPress: () => void;
};

export default function Button(props: ButtonProps) {
  return (
    <TouchableOpacity onPress={props.onPress}>
      <Text>{props.label}</Text>
    </TouchableOpacity>
  );
}
