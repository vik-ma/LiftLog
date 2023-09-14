import { TouchableOpacity, Text } from "react-native";

type ButtonProps = {
  label: string;
  styling: string;
  onPress: () => void;
};

export default function Button(props: ButtonProps) {
  return (
    <TouchableOpacity
      className={`${props.styling} px-4 pt-2.5 pb-3 rounded-lg`}
      onPress={props.onPress}
    >
      <Text className="text-xl text-white">{props.label}</Text>
    </TouchableOpacity>
  );
}
